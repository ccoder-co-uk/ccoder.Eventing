// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Services.Foundations;

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
                exception: ex,
                message: "Exception thrown whilst getting event auth info\n{Message}\n{StackTrace}",
                args: [ex.Message, ex.StackTrace]);

            throw;
        }
    }
}
