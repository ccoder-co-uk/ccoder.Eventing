using EventLibrary.Models;

namespace EventLibrary.Brokers;

internal interface IEventAuthorizationBroker
{
    IEventAuthInfo GetEventAuthInfo();
}
