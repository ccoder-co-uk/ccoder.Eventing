using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Processings;

public partial class EventProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject { Name = "test" }
        };

        await eventProcessingService.RaiseEventAsync(inputName, inputMessage);

        eventServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessage),
            Times.Once);
    }
}
