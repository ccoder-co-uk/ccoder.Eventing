using EventLibrary.Models;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        await eventOrchestrationService.RaiseEventAsync(inputName, inputMessage);

        eventServiceProviderServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessage),
            Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventsAsync()
    {
        string inputName = "event-name";
        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        await eventOrchestrationService.RaiseEventsAsync(inputName, inputMessages);

        eventServiceProviderServiceMock.Verify(
            service => service.RaiseEventsAsync(inputName, inputMessages),
            Times.Once);
    }
}
