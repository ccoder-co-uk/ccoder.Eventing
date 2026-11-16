// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Sql.Services.Processings;

namespace cCoder.Eventing.Sql;

internal sealed class SqlEventQueueHub(ISqlEventQueueProcessingService processingService)
    : ISqlEventQueueMonitor
{
    public Task<SqlEventQueueReport> GetQueueReportAsync(
        CancellationToken cancellationToken = default) =>
        processingService.GetQueueReportAsync(cancellationToken: cancellationToken);
}