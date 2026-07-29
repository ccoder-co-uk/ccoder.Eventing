// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Models.Validations;
using cCoder.Eventing.Apps.Services.Processings.Validations;

namespace cCoder.Eventing.Apps.Services.Orchestrations;

internal sealed partial class ChatOrchestrationService
{
    private static void ValidateSendChatMessage(
        ChatMessage newChatMessage,
        CancellationToken cancellationToken)
    {
        ValidationRule[] validationRules =
        [
            new ValidationRule
            {
                IsInvalid = () => newChatMessage is null,
                CreateException = () =>
                    new ArgumentNullException(
                        paramName: nameof(newChatMessage))
            },
            new ValidationRule
            {
                IsInvalid = () =>
                    string.IsNullOrWhiteSpace(
                        value: newChatMessage?.Text),
                CreateException = () =>
                    new ArgumentException(
                        message: "You must provide chat message text.",
                        paramName: nameof(newChatMessage))
            }
        ];

        cancellationToken.ThrowIfCancellationRequested();

        ValidationRulesEngine.Validate(
            validationRules: validationRules);
    }

    private static void ValidateReceiveChatMessage(
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