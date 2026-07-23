// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models.Exceptions;

internal sealed class ServiceValidationException(Exception innerException)
    : Exception("A validation error occurred.", innerException)
{
}