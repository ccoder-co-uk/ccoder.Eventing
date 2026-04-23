# cCoder.Eventing

`cCoder.Eventing` is the core in-process eventing library in this repository.

It exposes a single public entry point, `IEventHub`, for:

- listening to named events
- raising typed event messages
- flowing event auth information through the handler scope

## What it is for

Use this library when publishers and handlers run inside the same application and you want a lightweight event hub built on dependency injection.

Typical use cases:

- application-level domain events
- decoupling feature modules inside a service
- flowing user context through event handlers

## Public API

Consumers should go through `IEventHub`. The internal services are implementation details behind the hub.

## Registering the library

```csharp
using cCoder.Eventing;

builder.Services.AddEventing(serviceProvider =>
    serviceProvider.GetRequiredService<IHttpContextAccessor>()
        .HttpContext?
        .User?
        .Identity?
        ?.Name ?? "Guest");

builder.Services.AddEventingForType<Order>();
```

`AddEventing(...)` registers the shared eventing infrastructure.

`AddEventingForType<T>()` registers the handlers and services required for a specific event payload type.

## Listening for events

```csharp
using cCoder.Eventing;

public sealed class OrderEventHandler
{
    private readonly IEventHub eventHub;

    public OrderEventRegistrar(IEventHub eventHub) =>
        this.eventHub = eventHub;

    public void ListenToOrderEvents() 
    {
        eventHub.ListenToEvent<Order>(
            "orders.submitted",
            async (serviceProvider, order) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<OrderEventRegistrar>>();
                logger.LogInformation("Handled order {OrderId}", order.OrderId);

                await ValueTask.CompletedTask;
            });
    }
}
```

## Raising events

```csharp
using cCoder.Eventing;
using cCoder.Eventing.Models;

await eventHub.RaiseEventAsync(
    "orders.submitted",
    new EventMessage<Order>
    {
        AuthInfo = new EventAuthInfo { SSOUserId = "user-123" },
        Data = new Order { OrderId = "ORD-1001" }
    });
```

## Notes

- Event names are string based, so keep them stable and consistent.
- Event auth data is carried in `EventMessage<T>.AuthInfo`.
- If no handler is registered for a raised event, the hub logs a warning and does not throw.

## Related documentation

- [Repository root README](../../README.md)
- [cCoder.Eventing.AzureServiceBus README](../cCoder.Eventing.AzureServiceBus/README.md)
