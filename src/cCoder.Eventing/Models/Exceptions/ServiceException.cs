// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Models.Exceptions;

internal sealed class ServiceException(Exception innerException)
    : Exception("A service error occurred.", innerException)
{
}