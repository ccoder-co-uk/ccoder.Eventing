// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Orchestrations;

namespace cCoder.Eventing;

public class EventHub : IEventHub
{
    private readonly IEventOrchestrationService eventOrchestrationService;

    internal EventHub(IEventOrchestrationService eventOrchestrationService) =>
        this.eventOrchestrationService = eventOrchestrationService;

    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        eventOrchestrationService.ListenToEvent(name, handler);

    public void ListenToEvent<T, TService>(string name, Func<TService, T, ValueTask> handler) =>
        eventOrchestrationService.ListenToEvent(name, handler);

    public ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        eventOrchestrationService.RaiseEventAsync(name, message);

    public ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages) =>
        eventOrchestrationService.RaiseEventsAsync(name, messages);
}