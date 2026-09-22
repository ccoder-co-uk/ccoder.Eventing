// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Http.Services.Processings;

using cCoder.Eventing.Models;

internal partial class HttpEventProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateCancellationToken(
        CancellationToken cancellationToken)
    {
    }

    private static void ValidateEventMessage<T>(
        string name,
        EventMessage<T> message,
        bool isConfigured)
    {
        if (string.IsNullOrWhiteSpace(value: name))
        {
            throw new ArgumentException(
                message: "You must provide an event name when raising events.",
                paramName: nameof(name));
        }

        if (message.Data is null)
        {
            throw new ArgumentException(
                message: "You must provide some message data when raising events.",
                paramName: nameof(message));
        }

        if (message.AuthInfo is null)
        {
            throw new ArgumentException(
                message: "You must provide some message auth information when raising events.",
                paramName: nameof(message));
        }

        if (!isConfigured)
        {
            throw new InvalidOperationException(
                "You must provide an HTTP event hub URL before sending events.");
        }
    }
}