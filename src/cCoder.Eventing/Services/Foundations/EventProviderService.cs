// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;
using cCoder.Eventing.Brokers.Loggings;

namespace cCoder.Eventing.Services.Foundations;

internal sealed partial class EventProviderService(
        IServiceProviderBroker serviceProviderBroker,
        ILoggingBroker log)
            : IEventProviderService
{
    public ValueTask<bool> RaiseEventAsync<T>(
        string name,
        EventMessage<T> message) =>
        TryCatch<bool>(operation: async () =>
        {
            Validate(inputs: [name, message]);

            try
            {
            ValidateRequest(name:name, message:message);

            EventProvider[] matchingProviders = serviceProviderBroker
                .GetServices<EventProvider>()
                .Where(predicate:provider => provider.CanSend<T>(name:name))
                .ToArray();

            if (matchingProviders.Length == 0)
            {
                return false;
            }

            using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(message:message);

            foreach (EventProvider provider in matchingProviders)
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

    public ValueTask<bool> RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages) =>
        TryCatch<bool>(operation: async () =>
        {
            Validate(inputs: [name, messages]);

            try
            {
            ValidateRequest(name:name, messages:messages);

            BulkEventProvider[] matchingProviders = serviceProviderBroker
                .GetServices<BulkEventProvider>()
                .Where(predicate:provider => provider.CanHandle<T>(name:name))
                .ToArray();

            if (matchingProviders.Length == 0 || messages.Length == 0)
            {
                return false;
            }

            using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(message:messages[0]);

            foreach (BulkEventProvider provider in matchingProviders)
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

        Array.ForEach(
            array: messages,
            action: message => ValidateRequest(
                name: name,
                message: message));
    }
}