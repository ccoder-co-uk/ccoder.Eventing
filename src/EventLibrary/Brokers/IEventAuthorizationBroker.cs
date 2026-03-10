using EventLibrary.Models.Interfaces;

namespace EventLibrary.Brokers.Interfaces;

public interface IEventAuthorizationBroker
{
    IEventAuthInfo GetEventAuthInfo();
    string GetSSOUserId();
}
