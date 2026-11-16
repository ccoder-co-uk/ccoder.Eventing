// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;

namespace cCoder.Eventing.Sql.Services.Processings;

internal sealed partial class SqlEventProcessingService
{
    private static void ValidateFailedOnGet(int count) =>
        Validate(inputs: [count]);

    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }
}