// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Eventing.Sql.Models.Exceptions;

namespace cCoder.Eventing.Sql.Services.Processings;

internal sealed partial class SqlEventQueueProcessingService
{
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
        catch (Exception innerException)
        {
            throw new ServiceException(innerException);
        }
    }
}