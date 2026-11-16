// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Sql.Models.Exceptions;

internal sealed class ServiceValidationException(Exception innerException)
    : Exception("A SQL event validation error occurred.", innerException)
{
}