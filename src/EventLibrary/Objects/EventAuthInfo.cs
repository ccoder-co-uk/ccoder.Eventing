using EventLibrary.Objects.Interfaces;

namespace EventLibrary.Objects
{
    public class EventAuthInfo : IEventAuthInfo
    {
        public string SSOUserId { get; set; }
    }
}