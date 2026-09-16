// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Models;
using cCoder.Eventing.Brokers;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Http.Services.Foundations;

internal sealed partial class HttpEventService(
    IHttpEventBroker httpEventBroker,
    IServiceProviderBroker serviceProviderBroker) : IHttpEventService
{
    public bool IsConfigured() =>
        TryCatch(operation: () =>
        {
            return httpEventBroker.IsConfigured();
        });

    public string Serialize(object value) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [value]);

            return httpEventBroker.Serialize(value: value);
        });

    public ValueTask SendHttpEventMessageAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [httpEventMessage, cancellationToken]);
            ValidateHttpEventMessage(httpEventMessage: httpEventMessage);
            ValidateConfiguration(isConfigured: httpEventBroker.IsConfigured());

            return httpEventBroker.SendAsync(
                httpEventMessage: httpEventMessage,
                cancellationToken: cancellationToken);
        });

    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);
            ValidateEventName(name: name);

            httpEventBroker.InsertSubscription(
                name: name,
                handler: handler);
        });

    public ValueTask EnqueueHttpEventMessageAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [httpEventMessage, cancellationToken]);
            ValidateHttpEventMessage(httpEventMessage: httpEventMessage);

            return httpEventBroker.EnqueueAsync(
                httpEventMessage: httpEventMessage,
                cancellationToken: cancellationToken);
        });

    public int GetMaxConcurrency() =>
        TryCatch(operation: () =>
        {
            return httpEventBroker.SelectMaxConcurrency();
        });

    public IAsyncEnumerable<HttpEventMessage> ReadAllHttpEventMessagesAsync(
        CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [cancellationToken]);

            return httpEventBroker.ReadAllAsync(
                cancellationToken: cancellationToken);
        });

    public ValueTask<bool> DispatchHttpEventMessageAsync(
        HttpEventMessage httpEventMessage) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [httpEventMessage]);

            IReadOnlyCollection<HttpEventSubscription> subscriptions =
                httpEventBroker.SelectSubscriptions(
                    name: httpEventMessage.EventName);

            foreach (HttpEventSubscription httpEventSubscription
                in subscriptions)
            {
                using IServiceScope scope = serviceProviderBroker
                    .GetScopeForEvent(
                        eventMessage: CreateScopeEventMessage(
                            httpEventMessage: httpEventMessage));

                object data = httpEventBroker.Deserialize(
                    httpEventSubscription: httpEventSubscription,
                    httpEventMessage: httpEventMessage);

                await httpEventSubscription.Handler(
                    arg1: scope.ServiceProvider,
                    arg2: data);
            }

            IReadOnlyCollection<HttpEventProviderBrokerConfiguration>
                httpEventProviderBrokerConfigurations =
                    httpEventBroker.SelectEventProviderConfigurations(
                        name: httpEventMessage.EventName);

            foreach (HttpEventProviderBrokerConfiguration
                httpEventProviderBrokerConfiguration in
                httpEventProviderBrokerConfigurations)
            {
                EventMessage eventMessage = httpEventBroker.CreateMessage(
                    httpEventProviderBrokerConfiguration:
                        httpEventProviderBrokerConfiguration,
                    httpEventMessage: httpEventMessage);

                using IServiceScope scope = serviceProviderBroker
                    .GetScopeForEvent(eventMessage: eventMessage);

                await httpEventBroker.HandleAsync(
                    httpEventProviderBrokerConfiguration:
                        httpEventProviderBrokerConfiguration,
                    serviceProvider: scope.ServiceProvider,
                    eventName: httpEventMessage.EventName,
                    eventMessage: eventMessage);
            }

            return subscriptions.Count > 0
                || httpEventProviderBrokerConfigurations.Count > 0;
        });

    private static EventMessage<object> CreateScopeEventMessage(
        HttpEventMessage httpEventMessage) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = httpEventMessage.SSOUserId
            },
            Data = new object()
        };

    private static void ValidateConfiguration(bool isConfigured)
    {
        if (!isConfigured)
        {
            throw new InvalidOperationException(
                "You must provide an HTTP event hub URL before sending events.");
        }
    }
}