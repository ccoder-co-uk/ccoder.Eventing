// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Http.Services.Foundations;

using cCoder.Eventing.Http.Models;

internal sealed partial class HttpEventService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateOperation()
    {
    }

    private static void ValidateEventName(string name)
    {
        if (string.IsNullOrWhiteSpace(value: name))
        {
            throw new ArgumentException(
                message: "You must provide an event name when listening for events.",
                paramName: nameof(name));
        }
    }

    private static void ValidateHttpEventMessage(
        HttpEventMessage httpEventMessage)
    {
        if (string.IsNullOrWhiteSpace(value: httpEventMessage.EventName))
        {
            throw new ArgumentException(
                message: "You must provide an event name when receiving events.",
                paramName: nameof(httpEventMessage));
        }

        if (string.IsNullOrWhiteSpace(value: httpEventMessage.Data))
        {
            throw new ArgumentException(
                message: "You must provide message data when receiving events.",
                paramName: nameof(httpEventMessage));
        }
    }

    private static void ValidateCancellationToken(
        CancellationToken cancellationToken)
    {
    }
}