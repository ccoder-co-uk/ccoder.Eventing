// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Sql;

public interface ISqlEventRetryContext
{
    void RequestRetry(TimeSpan delay);
}