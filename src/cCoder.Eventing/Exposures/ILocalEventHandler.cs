// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace cCoder.Eventing;

public interface ILocalEventHandler<in T>
{
    ValueTask HandleAsync(T data);
}