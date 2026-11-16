// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Eventing.Sql.Models.Exceptions;

namespace cCoder.Eventing.Sql.Services.Processings;

internal sealed partial class SqlEventProcessingService
{
    private static async Task TryCatch(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new ServiceValidationException(innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ServiceDependencyException(innerException);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception innerException)
        {
            throw new ServiceException(innerException);
        }
    }

    private static async Task<T> TryCatch<T>(Func<Task<T>> operation)
    {
        try
        {
            return await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new ServiceValidationException(innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ServiceDependencyException(innerException);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception innerException)
        {
            throw new ServiceException(innerException);
        }
    }

    private static async ValueTask<T> TryCatch<T>(Func<ValueTask<T>> operation)
    {
        try
        {
            return await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new ServiceValidationException(innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ServiceDependencyException(innerException);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception innerException)
        {
            throw new ServiceException(innerException);
        }
    }
}