using EventLibrary.Models;

namespace EventLibrary.Brokers;

public class EventAuthorizationBroker : IEventAuthorizationBroker
{
    private readonly Func<string> getSSOUserId;

    internal EventMessage Message { get; set; }

    public EventAuthorizationBroker(Func<string> getSSOUserId) =>
        this.getSSOUserId = getSSOUserId;

    public IEventAuthInfo GetEventAuthInfo() =>
        Message?.AuthInfo ?? new EventAuthInfo { SSOUserId = GetSSOUserId() };

    public string GetSSOUserId() =>
        Message?.AuthInfo.SSOUserId ?? getSSOUserId();
}
