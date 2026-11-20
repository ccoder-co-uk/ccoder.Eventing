// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.Http.Brokers.Loggings;

public interface ILoggingBroker : IUtilityBroker
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogWarning(Exception exception, string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}