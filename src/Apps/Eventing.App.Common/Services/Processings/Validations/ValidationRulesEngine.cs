// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models.Validations;

namespace cCoder.Eventing.Apps.Services.Processings.Validations;

internal static class ValidationRulesEngine
{
    internal static void Validate(
        IEnumerable<ValidationRule> validationRules)
    {
        ValidationRule? invalidValidationRule =
            validationRules.FirstOrDefault(
                predicate: validationRule =>
                    validationRule.IsInvalid?.Invoke() == true);

        if (invalidValidationRule is not null)
        {
            throw invalidValidationRule.CreateException?.Invoke()
                ?? new ArgumentException(
                    message: "A validation rule failed.");
        }
    }
}