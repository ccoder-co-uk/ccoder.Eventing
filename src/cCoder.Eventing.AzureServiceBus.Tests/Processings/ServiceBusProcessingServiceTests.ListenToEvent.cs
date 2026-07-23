// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceBusServiceMock
            .Setup(expression:service => service.ListenToEvent(
                inputName,
                inputHandler));

        serviceBusProcessingService.ListenToEvent(name:inputName, handler:inputHandler);

        serviceBusServiceMock.Verify(
expression:            service => service.ListenToEvent(inputName, inputHandler),
times:            Times.Once);
    }
}