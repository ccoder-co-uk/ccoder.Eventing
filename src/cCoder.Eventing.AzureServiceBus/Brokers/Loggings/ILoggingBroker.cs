// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.Brokers.Loggings;

internal interface ILoggingBroker
{
    void LogError(Exception exception, string message, params object[] args);
}