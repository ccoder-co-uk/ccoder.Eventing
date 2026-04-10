namespace EventLibrary.Models;

public abstract class EventProvider
{
    public string[] Events { get; set; } = [];

    internal abstract Type MessageType { get; }

    internal bool CanHandle<T>(string name) =>
        Events?.Contains(name, StringComparer.Ordinal) == true &&
        MessageType == typeof(T);

    internal abstract ValueTask HandleAsync(IServiceProvider serviceProvider, EventMessage message);
}

public class EventProvider<T> : EventProvider
{
    public Func<IServiceProvider, EventMessage<T>, ValueTask> Handler { get; set; }

    internal override Type MessageType => typeof(T);

    internal override ValueTask HandleAsync(IServiceProvider serviceProvider, EventMessage message)
    {
        if (Handler is null)
        {
            throw new InvalidOperationException(
                $"You must provide a handler for event providers of type {typeof(T).Name}.");
        }

        return Handler(serviceProvider, (EventMessage<T>)message);
    }
}
