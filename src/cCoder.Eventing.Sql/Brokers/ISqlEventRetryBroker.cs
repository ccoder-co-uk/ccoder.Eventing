// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.Sql.Brokers;

internal interface ISqlEventRetryBroker : IUtilityBroker
{
    TimeSpan? GetRequestedDelay();
    void Reset();
}