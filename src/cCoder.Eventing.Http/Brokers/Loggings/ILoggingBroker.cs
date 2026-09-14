// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.Http.Brokers.Loggings;

public interface ILoggingBroker : IUtilityBroker
{
    void LogError(Exception exception, string message, params object[] args);
}