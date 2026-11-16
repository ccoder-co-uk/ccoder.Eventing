// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.Sql.Brokers;

internal interface ISqlEventLoggingBroker : IUtilityBroker
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}