// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Eventing.Http.Models.Exceptions;

namespace cCoder.Eventing.Http.Services.Foundations;

internal sealed partial class HttpEventQueueService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (ArgumentException innerException)
        {
            throw new ServiceValidationException(innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ServiceDependencyException(innerException);
        }
        catch (Exception innerException)
        {
            throw new ServiceException(innerException);
        }
    }
}