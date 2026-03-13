using EventLibrary.Brokers;
using EventLibrary.Models;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Foundations;

internal class EventAuthorizationService : IEventAuthorizationService
{
    private readonly IEventAuthorizationBroker authInfoBroker;
    private readonly ILogger<EventAuthorizationService> log;

    public EventAuthorizationService(
        IEventAuthorizationBroker authInfoBroker,
        ILogger<EventAuthorizationService> log)
    {
        this.authInfoBroker = authInfoBroker;
        this.log = log;
    }

    public IEventAuthInfo GetEventAuthInfo()
    {
        try
        {
            return authInfoBroker.GetEventAuthInfo();
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst getting event auth info\n{Message}\n{StackTrace}",
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }
}
