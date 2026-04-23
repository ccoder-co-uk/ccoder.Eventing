using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Foundations;

internal interface IEventAuthorizationService
{
    IEventAuthInfo GetEventAuthInfo();
}
