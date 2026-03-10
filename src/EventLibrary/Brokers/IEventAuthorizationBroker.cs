using EventLibrary.Models;

namespace EventLibrary.Brokers;

public interface IEventAuthorizationBroker
{
    IEventAuthInfo GetEventAuthInfo();
    string GetSSOUserId();
}