using EventLibrary.Models;
using EventLibrary.Models.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Processings;

public partial class EventProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        string inputName = "event-name";
        FakeObject inputData = new() { Name = "test" };
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = inputData
        };

        IServiceProvider scopedServiceProvider = Mock.Of<IServiceProvider>();
        Mock<IServiceScope> serviceScopeMock = new();

        serviceScopeMock
            .SetupGet(scope => scope.ServiceProvider)
            .Returns(scopedServiceProvider);

        serviceProviderBrokerMock
            .Setup(broker => broker.GetScopeForEvent(inputMessage))
            .Returns(serviceScopeMock.Object);

        await eventProcessingService.RaiseEventAsync(inputName, inputMessage);

        serviceProviderBrokerMock.Verify(
            broker => broker.GetScopeForEvent(inputMessage),
            Times.Once);

        eventServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, scopedServiceProvider, inputMessage),
            Times.Once);
    }
}
