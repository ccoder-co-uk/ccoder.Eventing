namespace EventLibrary.Models;

public abstract class BulkEventProvider
{
    public string[] Events { get; set; } = [];

    internal abstract Type MessageType { get; }

    internal bool CanHandle<T>(string name) =>
        Events?.Contains(name, StringComparer.Ordinal) == true &&
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

        return Handler(serviceProvider, (EventMessage<T>[])messages);
    }
}
