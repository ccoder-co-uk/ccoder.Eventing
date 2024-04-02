using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects.Interfaces;
using EventLibrary.Services.Foundation.Interfaces;

namespace EventLibrary.Services.Foundation
{
    public class EventAuthorizationService : IEventAuthorizationService
    {
        private readonly IEventAuthorizationBroker authInfoBroker;

        public EventAuthorizationService(IEventAuthorizationBroker authInfoBroker) =>
            this.authInfoBroker = authInfoBroker;

        public IEventAuthInfo GetEventAuthInfo() =>
            authInfoBroker.GetEventAuthInfo();
    }
}
