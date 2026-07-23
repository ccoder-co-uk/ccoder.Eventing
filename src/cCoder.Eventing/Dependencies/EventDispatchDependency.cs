// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Dependencies;

internal static class EventDispatchDependency
{
    internal static async ValueTask HandleSendAsync<T>(
        EventProvider[] providers,
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage<T> message)
    {
        foreach (EventProvider provider in providers)
        {
            await provider.HandleSendAsync(
                serviceProvider: serviceProvider,
                eventName: eventName,
                message: message);
        }
    }

    internal static async ValueTask HandleBulkAsync<T>(
        BulkEventProvider[] providers,
        IServiceProvider serviceProvider,
        EventMessage<T>[] messages)
    {
        foreach (BulkEventProvider provider in providers)
        {
            await provider.HandleAsync(
                serviceProvider: serviceProvider,
                messages: messages);
        }
    }

    internal static async ValueTask HandleHandlersAsync<T>(
        IEnumerable<Func<IServiceProvider, T, ValueTask>> handlers,
        IServiceProvider serviceProvider,
        T message)
    {
        foreach (Func<IServiceProvider, T, ValueTask> handler in handlers)
        {
            await handler.Invoke(
                arg1: serviceProvider,
                arg2: message);
        }
    }

    internal static async ValueTask HandleMessagesAsync<T>(
        EventMessage<T>[] messages,
        Func<EventMessage<T>, ValueTask> handler)
    {
        foreach (EventMessage<T> message in messages)
        {
            await handler(arg: message);
        }
    }
}