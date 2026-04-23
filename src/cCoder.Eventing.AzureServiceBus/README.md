# cCoder.Eventing.AzureServiceBus

`cCoder.Eventing.AzureServiceBus` provides an Azure Service Bus backed implementation of `IEventHub`.

It keeps the same core eventing contract as `cCoder.Eventing`, but publishes events to Azure Service Bus instead of dispatching them to in-process listeners.

## What it is for

Use this library when:

- producers and consumers live in different processes or services
- you want to publish through Azure Service Bus
- you still want application code to depend on `IEventHub`

## Registering the library

```csharp
using cCoder.Eventing.AzureServiceBus;

builder.Services.AddAzureServiceBusEventing(
    serviceBusConnectionString: builder.Configuration.GetConnectionString("ServiceBus"),
    getUserId: serviceProvider => "some-user-id");
```

This registration wires up:

- `IEventHub`
- `IAzureServiceBusEventHub`
- `IServiceBusBroker`
- `IServiceBusService`
- `IServiceBusProcessingService`

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

## Important behavior

- The Azure Service Bus hub is a pass-through entry point.
- Message validation and message creation happen in processing/foundation services inside this library.
- `RaiseEventsAsync(...)` loops over `RaiseEventAsync(...)` calls.
- In-process event listeners are not supported by the Azure Service Bus implementation.

## Notes

- The event name is used as the sender target name.
- Message IDs include the SSO user id, payload type name, and a generated GUID.
- Authentication context still flows through `EventMessage<T>.AuthInfo`.

## Related documentation

- [Repository root README](../../README.md)
- [cCoder.Eventing README](../cCoder.Eventing/README.md)
