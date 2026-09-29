// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Http.Models.Exceptions;

internal sealed class ServiceDependencyException(Exception innerException)
    : Exception("A dependency error occurred.", innerException)
{
}