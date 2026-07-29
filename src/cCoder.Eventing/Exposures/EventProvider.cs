// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public abstract class EventProvider
{
    public string[] Events { get; set; } = [];

    internal abstract Type MessageType { get; }

    public Type DataType => MessageType;

    public bool CanReceive(string name) =>
        Events?.Contains(value:name, comparer:StringComparer.Ordinal) == true &&
        HasReceiveHandler;

    public ValueTask ReceiveAsync(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage message) =>
        HandleReceiveAsync(serviceProvider:serviceProvider, eventName:eventName, message:message);

    internal bool CanSend<T>(string name) =>
        Events?.Contains(value:name, comparer:StringComparer.Ordinal) == true &&
        MessageType == typeof(T) &&
        HasSendHandler;

    internal bool CanReceive<T>(string name) =>
        Events?.Contains(value:name, comparer:StringComparer.Ordinal) == true &&
        MessageType == typeof(T) &&
        HasReceiveHandler;

    internal abstract bool HasSendHandler { get; }

    internal abstract bool HasReceiveHandler { get; }

    internal abstract ValueTask HandleSendAsync(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage message);

    internal abstract ValueTask HandleReceiveAsync(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage message);
}