using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Services.Foundations;

internal class EventProviderService(
        IServiceProviderBroker serviceProviderBroker,
        IEnumerable<EventProvider> eventProviders,
        IEnumerable<BulkEventProvider> bulkEventProviders,
        ILogger<EventProviderService> log)
            : IEventProviderService
{
    public async ValueTask<bool> RaiseEventAsync<T>(string name, EventMessage<T> message)
    {
        try
        {
            ValidateRequest(name, message);

            EventProvider[] matchingProviders = eventProviders
                .Where(provider => provider.CanSend<T>(name))
                .ToArray();

            if (matchingProviders.Length == 0)
            {
                return false;
            }

            using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(message);

            foreach (EventProvider provider in matchingProviders)
            {
                await provider.HandleSendAsync(scope.ServiceProvider, name, message);
            }

            return true;
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst raising {Name} event provider\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }

    public async ValueTask<bool> RaiseEventsAsync<T>(string name, EventMessage<T>[] messages)
    {
        try
        {
            ValidateRequest(name, messages);

            BulkEventProvider[] matchingProviders = bulkEventProviders
                .Where(provider => provider.CanHandle<T>(name))
                .ToArray();

            if (matchingProviders.Length == 0 || messages.Length == 0)
            {
                return false;
            }

            using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(messages[0]);

            foreach (BulkEventProvider provider in matchingProviders)
            {
                await provider.HandleAsync(scope.ServiceProvider, messages);
            }

            return true;
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst raising {Name} bulk event provider\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }

    private static void ValidateRequest<T>(string name, EventMessage<T> message)
    {
        if (name is null)
        {
            throw new InvalidOperationException("You must provide an event name when raising events.");
        }

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

        foreach (EventMessage<T> message in messages)
        {
            ValidateRequest(name, message);
        }
    }
}
