using EventLibrary.Models;

namespace EventLibrary.Services.Foundations;

public interface IEventAuthorizationService
{
    IEventAuthInfo GetEventAuthInfo();
}
