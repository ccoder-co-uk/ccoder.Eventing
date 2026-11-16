// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Sql.Brokers;

namespace cCoder.Eventing.Sql.Services.Foundations;

internal sealed partial class SqlEventQueueService(ISqlEventStore store)
    : ISqlEventQueueService
{
    public Task<SqlEventQueueReport> GetQueueReportAsync(CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            ValidateQueueReportOnGet(cancellationToken: cancellationToken);
            return store.GetQueueReportAsync(cancellationToken: cancellationToken);
        });
}