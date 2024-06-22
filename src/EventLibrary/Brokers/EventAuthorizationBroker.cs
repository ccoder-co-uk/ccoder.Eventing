using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Objects.Interfaces;

namespace EventLibrary.Brokers
{
    /// <summary>
    /// 
    /// </summary>
    public class EventAuthorizationBroker : IEventAuthorizationBroker
    {
        readonly Func<string> getSSOUserId;

        internal EventMessage Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getSSOUserId"></param>
        public EventAuthorizationBroker(Func<string> getSSOUserId) =>
            this.getSSOUserId = getSSOUserId;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEventAuthInfo GetEventAuthInfo() =>
            Message?.AuthInfo ?? new EventAuthInfo { SSOUserId = GetSSOUserId() };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSSOUserId() =>
            Message?.AuthInfo.SSOUserId ?? getSSOUserId();
    }
}