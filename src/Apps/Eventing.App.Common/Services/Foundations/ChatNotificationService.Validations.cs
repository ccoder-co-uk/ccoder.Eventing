// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Models.Validations;
using cCoder.Eventing.Apps.Services.Processings.Validations;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal sealed partial class ChatNotificationService
{
    private static void ValidateSendChatMessage(
        ChatMessage chatMessage)
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

        ValidationRulesEngine.Validate(
            validationRules: validationRules);
    }
}