// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;

namespace cCoder.Eventing.Sql;

public interface ISqlEventQueueMonitor
{
    Task<SqlEventQueueReport> GetQueueReportAsync(CancellationToken cancellationToken = default);
}