// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;

namespace cCoder.Eventing.Sql.Services.Foundations;

internal sealed partial class SqlEventQueueService
{
    private static void ValidateQueueReportOnGet(CancellationToken cancellationToken) =>
        Validate(inputs: [cancellationToken]);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}