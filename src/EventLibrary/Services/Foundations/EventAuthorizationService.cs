using EventLibrary.Brokers.Interfaces;
using EventLibrary.Models.Interfaces;

namespace EventLibrary.Services.Foundations;

public class EventAuthorizationService : IEventAuthorizationService
{
    private readonly IEventAuthorizationBroker authInfoBroker;

    public EventAuthorizationService(IEventAuthorizationBroker authInfoBroker) =>
        this.authInfoBroker = authInfoBroker;

    public IEventAuthInfo GetEventAuthInfo() =>
        authInfoBroker.GetEventAuthInfo();
}
