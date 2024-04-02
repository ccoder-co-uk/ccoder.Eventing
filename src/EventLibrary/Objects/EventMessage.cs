using EventLibrary.Objects.Interfaces;

namespace EventLibrary.Objects
{
    public class EventMessage<T> : EventMessage
    {
        public T Data { get; set; }
    }

    public abstract class EventMessage
    {
        public IEventAuthInfo AuthInfo { get; set; }
    }
}