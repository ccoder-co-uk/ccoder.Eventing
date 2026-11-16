// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;

namespace cCoder.Eventing.Sql.Services.Processings;

internal sealed partial class SqlEventQueueProcessingService
{
    private static void ValidateQueueReportOnGet(CancellationToken cancellationToken) =>
        Validate(inputs: [cancellationToken]);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}