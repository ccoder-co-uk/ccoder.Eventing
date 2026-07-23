// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Foundations;

internal class HttpEventHandlerRegistry : IHttpEventHandlerRegistry
{
    private readonly List<HttpEventSubscription> subscriptions = [];

    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler)
    {
        if (string.IsNullOrWhiteSpace(value:name))
            throw new InvalidOperationException("You must provide an event name when listening for events.");

        if (handler is null)
            throw new InvalidOperationException("You must provide a handler when listening for events.");

        lock (subscriptions)
        {
            subscriptions.Add(item:new HttpEventSubscription
            {
                EventName = name,
                DataType = typeof(T),
                Handler = (serviceProvider, data) => handler(arg1:serviceProvider, arg2:(T)data)
            });
        }
    }

    public IReadOnlyCollection<HttpEventSubscription> GetSubscriptions(string name)
    {
        lock (subscriptions)
        {
            return subscriptions
                .Where(predicate:subscription => subscription.EventName == name)
                .ToArray();
        }
    }
}