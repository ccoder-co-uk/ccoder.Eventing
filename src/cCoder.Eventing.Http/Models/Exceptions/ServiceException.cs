// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Http.Models.Exceptions;

internal sealed class ServiceException(Exception innerException)
    : Exception("A service error occurred.", innerException)
{
}