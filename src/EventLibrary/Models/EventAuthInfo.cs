using EventLibrary.Models.Interfaces;

namespace EventLibrary.Models;

public class EventAuthInfo : IEventAuthInfo
{
    public string SSOUserId { get; set; }
}
