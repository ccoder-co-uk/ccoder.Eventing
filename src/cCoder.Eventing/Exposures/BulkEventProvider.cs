// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public abstract class BulkEventProvider
{
    public string[] Events { get; set; } = [];

    internal abstract Type MessageType { get; }

    internal bool CanHandle<T>(string name) =>
        CanHandle(
            name: name,
            messageType: typeof(T));

    internal bool CanHandle(
        string name,
        Type messageType) =>
        Events?.Contains(value:name, comparer:StringComparer.Ordinal) == true &&
        MessageType == messageType;

    internal abstract ValueTask HandleAsync(IServiceProvider serviceProvider, Array messages);
}