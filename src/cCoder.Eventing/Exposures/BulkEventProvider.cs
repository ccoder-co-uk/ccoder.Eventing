// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public abstract class BulkEventProvider
{
    public string[] Events { get; set; } = [];

    internal abstract Type MessageType { get; }

    internal bool CanHandle<T>(string name) =>
        Events?.Contains(value:name, comparer:StringComparer.Ordinal) == true &&
        MessageType == typeof(T);

    internal abstract ValueTask HandleAsync(IServiceProvider serviceProvider, Array messages);
}