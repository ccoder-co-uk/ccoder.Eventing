// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;

namespace cCoder.Eventing.Dependencies;

internal static class ValidationRulesEngine
{
    internal static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate:input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }
}