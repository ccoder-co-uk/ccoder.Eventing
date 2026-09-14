// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Dependencies;

namespace cCoder.Eventing.Services.Processings;

internal sealed partial class BulkEventProviderProcessingService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}