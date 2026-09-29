// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.AzureServiceBus.Models.Exceptions;

internal sealed class ServiceException(Exception innerException)
    : Exception("A service error occurred.", innerException)
{
}