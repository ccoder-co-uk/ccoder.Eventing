// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Models.Validations;
using cCoder.Eventing.Apps.Services.Processings.Validations;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal sealed partial class ChatEventService
{
    private static void ValidateRaiseChatMessage(
        ChatMessage chatMessage,
        CancellationToken cancellationToken)
    {
        ValidationRule[] validationRules =
        [
            new ValidationRule
            {
                IsInvalid = () => chatMessage is null,
                CreateException = () =>
                    new ArgumentNullException(
                        paramName: nameof(chatMessage))
            }
        ];

        cancellationToken.ThrowIfCancellationRequested();

        ValidationRulesEngine.Validate(
            validationRules: validationRules);
    }
}