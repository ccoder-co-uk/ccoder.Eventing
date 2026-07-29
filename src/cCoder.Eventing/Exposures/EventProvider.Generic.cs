// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public class EventProvider<T> : EventProvider
{
    private Func<IServiceProvider, EventMessage<T>, ValueTask> handler;

    public Func<IServiceProvider, string, EventMessage<T>, ValueTask> SendHandler { get; set; }

    public Func<IServiceProvider, string, EventMessage<T>, ValueTask> ReceiveHandler { get; set; }

    [Obsolete("Use SendHandler instead.")]
    public Func<IServiceProvider, EventMessage<T>, ValueTask> Handler
    {
        get => handler;
        set
        {
            handler = value;

            if (value is not null && SendHandler is null)
            {
                SendHandler = (serviceProvider, _, message) =>
                    value(arg1:serviceProvider, arg2:message);
            }
        }
    }

    internal override Type MessageType => typeof(T);

    internal override bool HasSendHandler => SendHandler is not null;

    internal override bool HasReceiveHandler => ReceiveHandler is not null;

    internal override ValueTask HandleSendAsync(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage message)
    {
        if (SendHandler is null)
        {
            throw new InvalidOperationException(
                $"You must provide a send handler for event providers of type {typeof(T)
                    .Name}.");
        }

        return SendHandler(
            arg1: serviceProvider,
            arg2: eventName,
            arg3: (EventMessage<T>)message);
    }

    internal override ValueTask HandleReceiveAsync(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage message)
    {
        if (ReceiveHandler is null)
        {
            throw new InvalidOperationException(
                $"You must provide a receive handler for event providers of type {typeof(T)
                    .Name}.");
        }

        return ReceiveHandler(
            arg1: serviceProvider,
            arg2: eventName,
            arg3: (EventMessage<T>)message);
    }
}