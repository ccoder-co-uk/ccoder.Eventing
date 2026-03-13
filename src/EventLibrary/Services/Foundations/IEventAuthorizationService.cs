using EventLibrary.Models;

namespace EventLibrary.Services.Foundations;

internal interface IEventAuthorizationService
{
    IEventAuthInfo GetEventAuthInfo();
}
