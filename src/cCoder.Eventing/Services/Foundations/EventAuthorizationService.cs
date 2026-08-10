// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Brokers.Loggings;

namespace cCoder.Eventing.Services.Foundations;

internal sealed partial class EventAuthorizationService : IEventAuthorizationService
{
    private readonly IEventAuthorizationBroker authInfoBroker;
    private readonly ILoggingBroker log;

    public EventAuthorizationService(
        IEventAuthorizationBroker authInfoBroker,
        ILoggingBroker log)
    {
        this.authInfoBroker = authInfoBroker;
        this.log = log;
    }

    public IEventAuthInfo GetEventAuthInfo() =>
        TryCatch(operation: () =>
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
        });
}