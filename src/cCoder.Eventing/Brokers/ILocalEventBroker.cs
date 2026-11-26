// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.Brokers;

/// <summary>Dispatches an event in the current scope, without external transport.</summary>
public interface ILocalEventBroker : IUtilityBroker
{
    IEnumerable<ILocalEventHandler<T>> GetHandlers<T>(string name);
}