// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Brokers.Loggings;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Services.Foundations;

internal sealed partial class EventProviderService(
        IEnumerable<IEventProviderBroker> eventProviderBrokers,
        IServiceProviderBroker serviceProviderBroker,
        ILoggingBroker log)
            : IEventProviderService
{
    public ValueTask<bool> RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        TryCatch<bool>(operation: async () =>
        {
            Validate(inputs: [name, message]);

            try
            {
                ValidateRequest(message: message);

                IEventProviderBroker[] matchingProviders = eventProviderBrokers
                    .Where(predicate: provider => provider.CanSend<T>(name: name))
                    .ToArray();

                if (matchingProviders.Length == 0)
                {
                    return false;
                }

                using IServiceScope scope = serviceProviderBroker
                    .GetScopeForEvent(eventMessage: message);

                foreach (IEventProviderBroker provider in matchingProviders)
                {
                    await provider.HandleSendAsync(
                        serviceProvider: scope.ServiceProvider,
                        eventName: name,
                        message: message);
                }

                return true;
            }
            catch (Exception ex)
            {
                log.LogError(
                    exception: ex,
                    message: "Exception thrown whilst raising {Name} event provider\n{Message}\n{StackTrace}",
                    args: [name, ex.Message, ex.StackTrace]);

                throw;
            }
        });

    private static void ValidateRequest<T>(EventMessage<T> message)
    {
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