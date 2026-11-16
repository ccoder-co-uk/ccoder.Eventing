// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Sql.Models;

internal sealed class SqlEventRetryState
{
    public TimeSpan? Delay { get; set; }
}