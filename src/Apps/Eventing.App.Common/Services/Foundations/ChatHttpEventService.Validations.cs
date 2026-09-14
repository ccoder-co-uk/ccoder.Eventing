// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Models.Validations;
using cCoder.Eventing.Apps.Services.Processings.Validations;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal sealed partial class ChatHttpEventService
{
    private static void ValidateRaiseChatMessage(
        EventMessage<ChatMessage> eventMessage,
        CancellationToken cancellationToken)
    {
        ValidationRule[] validationRules =
        [
            new ValidationRule
            {
                IsInvalid = () => eventMessage is null,
                CreateException = () =>
                    new ArgumentNullException(
                        paramName: nameof(eventMessage))
            }
        ];

        cancellationToken.ThrowIfCancellationRequested();

        ValidationRulesEngine.Validate(
            validationRules: validationRules);
    }
}