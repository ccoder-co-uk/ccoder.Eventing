// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Models.Exceptions;

internal sealed class ServiceDependencyException(Exception innerException)
    : Exception("A dependency error occurred.", innerException)
{
}