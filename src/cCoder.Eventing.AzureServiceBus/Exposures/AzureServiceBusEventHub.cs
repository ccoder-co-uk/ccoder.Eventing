// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Services.Processings;
using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus;

public class AzureServiceBusEventHub : IAzureServiceBusEventHub
{
    private readonly IServiceBusProcessingService serviceBusProcessingService;

    internal AzureServiceBusEventHub(IServiceBusProcessingService serviceBusProcessingService) =>
        this.serviceBusProcessingService = serviceBusProcessingService;

    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        serviceBusProcessingService.ListenToEvent(name:name, handler:handler);

    public ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> message) =>
        serviceBusProcessingService.RaiseEventAsync(name:name, message:message);

    public ValueTask RaiseEventsAsync<T>(string name, ServiceBusEventMessage<T>[] messages) =>
        serviceBusProcessingService.RaiseEventsAsync(name:name, messages:messages);
}