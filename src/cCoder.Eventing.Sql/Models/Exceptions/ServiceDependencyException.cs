// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Sql.Models.Exceptions;

internal sealed class ServiceDependencyException(Exception innerException)
    : Exception("A SQL event dependency error occurred.", innerException)
{
}