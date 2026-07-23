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

public class BulkEventProvider<T> : BulkEventProvider
{
    public Func<IServiceProvider, EventMessage<T>[], ValueTask> Handler { get; set; }

    internal override Type MessageType => typeof(T);

    internal override ValueTask HandleAsync(IServiceProvider serviceProvider, Array messages)
    {
        if (Handler is null)
        {
            throw new InvalidOperationException(
                $"You must provide a handler for bulk event providers of type {typeof(T).Name}.");
        }

        return Handler(arg1:serviceProvider, arg2:(EventMessage<T>[])messages);
    }
}