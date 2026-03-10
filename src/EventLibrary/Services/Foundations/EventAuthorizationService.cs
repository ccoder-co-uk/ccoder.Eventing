using EventLibrary.Brokers;
using EventLibrary.Models;

namespace EventLibrary.Services.Foundations;

public class EventAuthorizationService : IEventAuthorizationService
{
    private readonly IEventAuthorizationBroker authInfoBroker;

    public EventAuthorizationService(IEventAuthorizationBroker authInfoBroker) =>
        this.authInfoBroker = authInfoBroker;

    public IEventAuthInfo GetEventAuthInfo() =>
        authInfoBroker.GetEventAuthInfo();
}
