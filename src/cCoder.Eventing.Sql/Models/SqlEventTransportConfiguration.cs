// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Sql.Models;

internal sealed class SqlEventTransportConfiguration
{
    public SqlEventingOptions Options { get; set; }
    public EventProvider[] Providers { get; set; }
}