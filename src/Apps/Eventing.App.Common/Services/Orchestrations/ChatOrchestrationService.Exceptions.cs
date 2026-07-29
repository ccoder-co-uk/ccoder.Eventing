// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models.Exceptions;

namespace cCoder.Eventing.Apps.Services.Orchestrations;

internal sealed partial class ChatOrchestrationService
{
    private static async ValueTask TryCatch(
        Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new ChatServiceValidationException(
                innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ChatServiceDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ChatServiceException(
                innerException: innerException);
        }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new ChatServiceValidationException(
                innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new ChatServiceDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new ChatServiceException(
                innerException: innerException);
        }
    }
}