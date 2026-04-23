using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal interface IEventAuthorizationBroker
{
    IEventAuthInfo GetEventAuthInfo();
}
