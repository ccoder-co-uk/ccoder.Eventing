// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models.Exceptions;

internal sealed class ChatServiceException(
    Exception innerException)
    : Exception(
        message: "The chat operation failed.",
        innerException: innerException);