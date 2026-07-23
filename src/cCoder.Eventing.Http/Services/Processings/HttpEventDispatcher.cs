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
            .GetMethods(bindingAttr:BindingFlags.NonPublic | BindingFlags.Static)
            .Single(predicate:method =>
                method.Name == nameof(CreateMessage) &&
                method.IsGenericMethodDefinition);

    public async ValueTask DispatchAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool handledBySubscriptions = await DispatchToSubscriptionsAsync(message:message);
            bool handledByProviders = await DispatchToProvidersAsync(message:message);

            if (!handledBySubscriptions && !handledByProviders)
            {
                log.LogWarning(
message: "HTTP event {EventName} was received but no matching handlers were registered.",
args: message.EventName);
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
            eventHandlerRegistry.GetSubscriptions(name:message.EventName);

        foreach (HttpEventSubscription subscription in subscriptions)
        {
            object data = Deserialize(message:message, dataType:subscription.DataType);
            EventMessage eventMessage = CreateMessage(dataType:subscription.DataType, data:data, message:message);

            using IServiceScope scope =
                serviceProviderBroker.GetScopeForEvent(message:eventMessage);

            await subscription.Handler(arg1:scope.ServiceProvider, arg2:data);
        }

        return subscriptions.Count > 0;
    }

    private async ValueTask<bool> DispatchToProvidersAsync(HttpEventMessage message)
    {
        EventProvider[] matchingProviders = eventProviders
            .Where(predicate:provider => provider.CanReceive(name:message.EventName))
            .ToArray();

        foreach (EventProvider provider in matchingProviders)
        {
            object data = Deserialize(message:message, dataType:provider.DataType);
            EventMessage eventMessage = CreateMessage(dataType:provider.DataType, data:data, message:message);

            using IServiceScope scope =
                serviceProviderBroker.GetScopeForEvent(message:eventMessage);

            await provider.ReceiveAsync(
serviceProvider: scope.ServiceProvider,
eventName: message.EventName,
message: eventMessage);
        }

        return matchingProviders.Length > 0;
    }

    private object Deserialize(HttpEventMessage message, Type dataType) =>
        JsonSerializer.Deserialize(
json: message.Data,
returnType: dataType,
options: options.JsonSerializerOptions);

    private static EventMessage CreateMessage(
        Type dataType,
        object data,
        HttpEventMessage message) =>
            (EventMessage)CreateMessageMethod
                .MakeGenericMethod(typeArguments:dataType)
                .Invoke(obj:null, parameters:[data, message]);

    private static EventMessage<T> CreateMessage<T>(
        object data,
        HttpEventMessage message) =>
            new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = message.SSOUserId },
                Data = (T)data
            };
}