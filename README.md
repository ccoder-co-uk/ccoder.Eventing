# cCoder.Eventing

`cCoder.Eventing` provides two related libraries:

- `EventLibrary`
  Internal application eventing for in-process publish/subscribe workflows.
- `EventLibrary.AzureServiceBus`
  An Azure Service Bus backed `IEventHub` implementation for sending events out of process.

## Projects

- [EventLibrary](src/EventLibrary/README.md)
- [EventLibrary.AzureServiceBus](src/EventLibrary.AzureServiceBus/README.md)

## Which package to use

Use `EventLibrary` when event publishers and handlers live in the same application and you want a simple dependency-injection based event hub.

Use `EventLibrary.AzureServiceBus` when you want the same `IEventHub` abstraction to publish messages onto Azure Service Bus instead of dispatching them locally.

## Repository layout

- `src/EventLibrary`
  Core eventing abstractions, models, brokers, services, and the default `IEventHub`.
- `src/EventLibrary.AzureServiceBus`
  Azure Service Bus implementation of `IEventHub`.
- `src/EventLibrary.Tests`
  Unit tests for the core library.
- `src/EventLibrary.AzureServiceBus.Tests`
  Unit tests for the Azure Service Bus library.

## Getting started

For local in-process eventing, start with the [EventLibrary README](src/EventLibrary/README.md).

For Azure Service Bus publishing, start with the [EventLibrary.AzureServiceBus README](src/EventLibrary.AzureServiceBus/README.md).
