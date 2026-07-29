// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests;

internal sealed class AcceptanceTestConfiguration
{
    internal const string ConnectionStringVariableName =
        "Eventing__ServiceBus__ConnectionString";

    internal const string QueueNameVariableName =
        "Eventing__ServiceBus__AcceptanceQueueName";

    private AcceptanceTestConfiguration(
        string connectionString,
        string queueName)
    {
        ConnectionString = connectionString;
        QueueName = queueName;
    }

    internal string ConnectionString { get; }

    internal string QueueName { get; }

    internal bool IsComplete =>
        !string.IsNullOrWhiteSpace(value: ConnectionString)
        && !string.IsNullOrWhiteSpace(value: QueueName);

    internal static AcceptanceTestConfiguration Load() =>
        new(
            connectionString:
                ReadValue(variableName: ConnectionStringVariableName),
            queueName:
                ReadValue(variableName: QueueNameVariableName));

    private static string ReadValue(string variableName) =>
        Environment.GetEnvironmentVariable(variable: variableName)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.User)
        ?? Environment.GetEnvironmentVariable(
            variable: variableName,
            target: EnvironmentVariableTarget.Machine)
        ?? string.Empty;
}