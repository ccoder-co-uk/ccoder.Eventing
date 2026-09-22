// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Models;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading.Channels;

namespace cCoder.Eventing.Http.Brokers;

internal sealed class HttpEventBroker(
    IHttpClientFactory httpClientFactory,
    HttpEventBrokerConfiguration configuration) : IHttpEventBroker
{
    private static readonly MethodInfo CreateMessageMethod =
        typeof(HttpEventBroker)
            .GetMethods(bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
            .Single(predicate: method =>
                method.Name == nameof(CreateMessage)
                && method.IsGenericMethodDefinition);

    private readonly Channel<HttpEventMessage> channel =
        Channel.CreateUnbounded<HttpEventMessage>(
            options: new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

    private readonly List<HttpEventSubscription> subscriptions = [];

    public bool IsConfigured() =>
        !string.IsNullOrWhiteSpace(value: configuration.HubUrl);

    public int SelectMaxConcurrency() =>
        Math.Max(val1: 1, val2: configuration.MaxConcurrency);

    public string Serialize(object value) =>
        JsonSerializer.Serialize(
            value: value,
            options: configuration.JsonSerializerOptions);

    public async ValueTask SendAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = httpClientFactory.CreateClient(
            name: HttpEventingOptions.HttpClientName);

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            requestUri: configuration.HubUrl,
            value: httpEventMessage,
            options: configuration.JsonSerializerOptions,
            cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public ValueTask EnqueueAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default) =>
        channel.Writer.WriteAsync(
            item: httpEventMessage,
            cancellationToken: cancellationToken);

    public IAsyncEnumerable<HttpEventMessage> ReadAllAsync(
        CancellationToken cancellationToken = default) =>
        channel.Reader.ReadAllAsync(cancellationToken: cancellationToken);

    public void InsertSubscription<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler)
    {
        lock (subscriptions)
        {
            subscriptions.Add(item: new HttpEventSubscription
            {
                EventName = name,
                DataType = typeof(T),
                Handler = (serviceProvider, data) => handler(
                    arg1: serviceProvider,
                    arg2: (T)data)
            });
        }
    }

    public IReadOnlyCollection<HttpEventSubscription> SelectSubscriptions(
        string name)
    {
        lock (subscriptions)
        {
            return subscriptions
                .Where(predicate: subscription =>
                    subscription.EventName == name)
                .ToArray();
        }
    }

    public object Deserialize(
        HttpEventSubscription httpEventSubscription,
        HttpEventMessage httpEventMessage) =>
        JsonSerializer.Deserialize(
            json: httpEventMessage.Data,
            returnType: httpEventSubscription.DataType,
            options: configuration.JsonSerializerOptions);

    public IReadOnlyCollection<HttpEventProviderBrokerConfiguration>
        SelectEventProviderConfigurations(string name) =>
        configuration.EventProviderConfigurations
            .Where(predicate: eventProviderConfiguration =>
                eventProviderConfiguration.CanReceive(arg: name))
            .ToArray();

    public EventMessage CreateMessage(
        HttpEventProviderBrokerConfiguration
            httpEventProviderBrokerConfiguration,
        HttpEventMessage httpEventMessage)
    {
        object data = JsonSerializer.Deserialize(
            json: httpEventMessage.Data,
            returnType: httpEventProviderBrokerConfiguration.DataType,
            options: configuration.JsonSerializerOptions);

        return (EventMessage)CreateMessageMethod
            .MakeGenericMethod(
                typeArguments: httpEventProviderBrokerConfiguration.DataType)
            .Invoke(
                obj: null,
                parameters: [data, httpEventMessage]);
    }

    public ValueTask HandleAsync(
        HttpEventProviderBrokerConfiguration
            httpEventProviderBrokerConfiguration,
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage eventMessage) =>
        httpEventProviderBrokerConfiguration.ReceiveAsync(
            arg1: serviceProvider,
            arg2: eventName,
            arg3: eventMessage);

    private static EventMessage<T> CreateMessage<T>(
        object data,
        HttpEventMessage httpEventMessage) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = httpEventMessage.SSOUserId
            },
            Data = (T)data
        };
}