// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Sql.Brokers;

internal sealed class SqlEventLoggingBroker(ILogger<SqlEventLoggingBroker> logger)
    : ISqlEventLoggingBroker
{
    public void LogInformation(string message, params object[] args) =>
        logger.LogInformation(message: message, args: args);

    public void LogWarning(string message, params object[] args) =>
        logger.LogWarning(message: message, args: args);

    public void LogError(Exception exception, string message, params object[] args) =>
        logger.LogError(exception: exception, message: message, args: args);
}