// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Apps.Models.Exceptions;

internal sealed class ChatServiceDependencyException(
    Exception innerException)
    : Exception(
        message: "A chat dependency failed.",
        innerException: innerException);