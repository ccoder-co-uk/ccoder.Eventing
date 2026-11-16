// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;

namespace cCoder.Eventing.Sql.Services.Processings;

internal interface ISqlEventQueueProcessingService
{
    Task<SqlEventQueueReport> GetQueueReportAsync(CancellationToken cancellationToken);
}