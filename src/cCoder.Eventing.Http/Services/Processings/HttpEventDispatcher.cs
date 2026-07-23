// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace cCoder.Eventing.Http.Services.Processings;

internal class HttpEventDispatcher(
        IServiceProviderBroker serviceProviderBroker,
        IHttpEventHandlerRegistry eventHandlerRegistry,
        IEnumerable<EventProvider> eventProviders,
        HttpEventingOptions options,
        ILogger<HttpEventDispatcher> log)
            : IHttpEventDispatcher
{
    private static readonly MethodInfo CreateMessageMethod =
        typeof(HttpEventDispatcher)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(method =>
                method.Name == nameof(CreateMessage) &&
                method.IsGenericMethodDefinition);

    public async ValueTask DispatchAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool handledBySubscriptions = await DispatchToSubscriptionsAsync(message);
            bool handledByProviders = await DispatchToProvidersAsync(message);

            if (!handledBySubscriptions && !handledByProviders)
            {
                log.LogWarning(
                    "HTTP event {EventName} was received but no matching handlers were registered.",
                    message.EventName);
            }
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst dispatching HTTP event {EventName}: {Message}",
                message?.EventName,
                ex.Message);

            throw;
        }
    }

    private async ValueTask<bool> DispatchToSubscriptionsAsync(HttpEventMessage message)
    {
        IReadOnlyCollection<HttpEventSubscription> subscriptions =
            eventHandlerRegistry.GetSubscriptions(message.EventName);

        foreach (HttpEventSubscription subscription in subscriptions)
        {
            object data = Deserialize(message, subscription.DataType);
            EventMessage eventMessage = CreateMessage(subscription.DataType, data, message);

            using IServiceScope scope =
                serviceProviderBroker.GetScopeForEvent(eventMessage);

            await subscription.Handler(scope.ServiceProvider, data);
        }

        return subscriptions.Count > 0;
    }

    private async ValueTask<bool> DispatchToProvidersAsync(HttpEventMessage message)
    {
        EventProvider[] matchingProviders = eventProviders
            .Where(provider => provider.CanReceive(message.EventName))
            .ToArray();

        foreach (EventProvider provider in matchingProviders)
        {
            object data = Deserialize(message, provider.DataType);
            EventMessage eventMessage = CreateMessage(provider.DataType, data, message);

            using IServiceScope scope =
                serviceProviderBroker.GetScopeForEvent(eventMessage);

            await provider.ReceiveAsync(
                scope.ServiceProvider,
                message.EventName,
                eventMessage);
        }

        return matchingProviders.Length > 0;
    }

    private object Deserialize(HttpEventMessage message, Type dataType) =>
        JsonSerializer.Deserialize(
            message.Data,
            dataType,
            options.JsonSerializerOptions);

    private static EventMessage CreateMessage(
        Type dataType,
        object data,
        HttpEventMessage message) =>
            (EventMessage)CreateMessageMethod
                .MakeGenericMethod(dataType)
                .Invoke(null, [data, message]);

    private static EventMessage<T> CreateMessage<T>(
        object data,
        HttpEventMessage message) =>
            new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = message.SSOUserId },
                Data = (T)data
            };
}