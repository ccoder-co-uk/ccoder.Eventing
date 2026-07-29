// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models.Exceptions;

internal sealed class ChatServiceValidationException(
    Exception innerException)
    : Exception(
        message: "Chat validation failed.",
        innerException: innerException);