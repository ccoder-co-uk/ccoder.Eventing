// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Brokers.Loggings;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Services.Foundations;

internal sealed partial class BulkEventProviderService(
        IEnumerable<IBulkEventProviderBroker> eventProviderBrokers,
        IServiceProviderBroker serviceProviderBroker,
        ILoggingBroker log)
            : IBulkEventProviderService
{
    public ValueTask<bool> RaiseEventsAsync<T>(string name, EventMessage<T>[] messages) =>
        TryCatch<bool>(operation: async () =>
        {
            Validate(inputs: [name, messages]);

            try
            {
                ValidateRequest(name: name, messages: messages);

                IBulkEventProviderBroker[] matchingProviders = eventProviderBrokers
                    .Where(predicate: provider => provider.CanHandle<T>(name: name))
                    .ToArray();

                if (matchingProviders.Length == 0 || messages.Length == 0)
                {
                    return false;
                }

                using IServiceScope scope = serviceProviderBroker
                    .GetScopeForEvent(eventMessage: messages[0]);

                foreach (IBulkEventProviderBroker provider in matchingProviders)
                {
                    await provider.HandleAsync(
                        serviceProvider: scope.ServiceProvider,
                        messages: messages);
                }

                return true;
            }
            catch (Exception ex)
            {
                log.LogError(
                    exception: ex,
                    message: "Exception thrown whilst raising {Name} bulk event provider\n{Message}\n{StackTrace}",
                    args: [name, ex.Message, ex.StackTrace]);

                throw;
            }
        });

    private static void ValidateRequest<T>(string name, EventMessage<T>[] messages)
    {
        if (name is null)
        {
            throw new InvalidOperationException("You must provide an event name when raising events.");
        }

        if (messages is null)
        {
            throw new InvalidOperationException("You must provide a message collection when raising events.");
        }

        Array.ForEach(
            array: messages,
            action: message => ValidateMessage(message: message));
    }

    private static void ValidateMessage<T>(EventMessage<T> message)
    {
        if (message is null)
        {
            throw new InvalidOperationException("You must provide a message when raising events.");
        }

        if (message.Data is null)
        {
            throw new InvalidOperationException("You must provide some message data when raising events.");
        }

        if (message.AuthInfo is null)
        {
            throw new InvalidOperationException("You must provide some message auth information when raising events.");
        }
    }
}