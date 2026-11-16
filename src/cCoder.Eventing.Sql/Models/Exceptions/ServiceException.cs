// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Sql.Models.Exceptions;

internal sealed class ServiceException(Exception innerException)
    : Exception("A SQL event service error occurred.", innerException)
{
}