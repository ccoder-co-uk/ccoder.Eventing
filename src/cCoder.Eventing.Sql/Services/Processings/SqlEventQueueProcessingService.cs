// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Sql.Services.Foundations;

namespace cCoder.Eventing.Sql.Services.Processings;

internal sealed partial class SqlEventQueueProcessingService(ISqlEventQueueService service)
    : ISqlEventQueueProcessingService
{
    public Task<SqlEventQueueReport> GetQueueReportAsync(CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            ValidateQueueReportOnGet(cancellationToken: cancellationToken);
            return service.GetQueueReportAsync(cancellationToken: cancellationToken);
        });
}