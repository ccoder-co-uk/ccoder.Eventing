// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.Brokers.Loggings;

internal interface ILoggingBroker : IUtilityBroker
{
    void LogDebug(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
    void LogWarning(string message, params object[] args);
}