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
            },
            new ValidationRule
            {
                IsInvalid = () =>
                    string.IsNullOrWhiteSpace(
                        value: chatMessage?.Text),
                CreateException = () =>
                    new ArgumentException(
                        message: "You must provide chat message text.",
                        paramName: nameof(chatMessage))
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