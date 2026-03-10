using EventLibrary.Models.Interfaces;

namespace EventLibrary.Services.Foundations;

public interface IEventAuthorizationService
{
    IEventAuthInfo GetEventAuthInfo();
}
