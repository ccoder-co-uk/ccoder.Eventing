using EventLibrary.Objects.Interfaces;

namespace EventLibrary.Services.Foundation.Interfaces
{
    public interface IEventAuthorizationService
    {
        IEventAuthInfo GetEventAuthInfo();
    }
}