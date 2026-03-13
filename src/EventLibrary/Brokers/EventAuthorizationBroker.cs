using EventLibrary.Models;

namespace EventLibrary.Brokers;

internal class EventAuthorizationBroker : IEventAuthorizationBroker
{
    internal EventMessage Message { get; set; }

    public IEventAuthInfo GetEventAuthInfo() =>
        Message?.AuthInfo;
}
