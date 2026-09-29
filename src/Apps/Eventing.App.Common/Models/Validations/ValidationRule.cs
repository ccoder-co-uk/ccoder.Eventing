// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Apps.Models.Validations;

internal sealed class ValidationRule
{
    internal Func<bool>? IsInvalid { get; set; }

    internal Func<Exception>? CreateException { get; set; }
}