// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Models;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading.Channels;
using System.Collections.Concurrent;

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
        Channel.CreateBounded<HttpEventMessage>(
            options: new BoundedChannelOptions(
                capacity: Math.Max(val1: 1, val2: configuration.QueueCapacity))
            {
                SingleReader = false,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            });

    private readonly ConcurrentDictionary<string, HttpEventQueueCounters> queueCounters = new();

    private readonly List<HttpEventSubscription> subscriptions = [];

    public bool IsConfigured() =>
        !string.IsNullOrWhiteSpace(value: configuration.HubUrl);

    public int SelectMaxConcurrency() =>
        Math.Max(val1: 1, val2: configuration.MaxConcurrency);

    public IReadOnlyCollection<HttpEventQueueSnapshot> GetQueueSnapshots() =>
        queueCounters.Select(selector: pair => CreateSnapshot(
            name: pair.Key,
            counters: pair.Value))
            .OrderBy(keySelector: snapshot => snapshot.Name)
            .ToArray();

    public void RecordQueued(string name)
    {
        HttpEventQueueCounters counters = queueCounters.GetOrAdd(
            key: name,
            valueFactory: static _ => new HttpEventQueueCounters());

        Interlocked.Increment(location: ref counters.Queued);
    }

    public void RecordRejected(string name)
    {
        HttpEventQueueCounters counters = queueCounters.GetOrAdd(
            key: name,
            valueFactory: static _ => new HttpEventQueueCounters());

        Interlocked.Decrement(location: ref counters.Queued);
        Interlocked.Increment(location: ref counters.Failed);
    }

    public void RecordStarted(string name)
    {
        HttpEventQueueCounters counters = queueCounters.GetOrAdd(
            key: name,
            valueFactory: static _ => new HttpEventQueueCounters());

        Interlocked.Decrement(location: ref counters.Queued);
        Interlocked.Increment(location: ref counters.Processing);
    }

    public void RecordFinished(string name, bool succeeded)
    {
        HttpEventQueueCounters counters = queueCounters.GetOrAdd(
            key: name,
            valueFactory: static _ => new HttpEventQueueCounters());

        Interlocked.Decrement(location: ref counters.Processing);

        Interlocked.Add(
            location1: ref counters.Completed,
            value: Convert.ToInt32(value: succeeded));

        Interlocked.Add(
            location1: ref counters.Failed,
            value: 1 - Convert.ToInt32(value: succeeded));
    }

    private static HttpEventQueueSnapshot CreateSnapshot(
        string name,
        HttpEventQueueCounters counters) =>
        new()
        {
            Name = name,
            Queued = Interlocked.Read(location: ref counters.Queued),
            Processing = Interlocked.Read(location: ref counters.Processing),
            Completed = Interlocked.Read(location: ref counters.Completed),
            Failed = Interlocked.Read(location: ref counters.Failed)
        };

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