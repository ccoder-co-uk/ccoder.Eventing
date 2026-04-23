using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventProviderServiceTests
{
    [Fact]
    public async Task ShouldRethrowOnRaiseEventAsyncIfProviderFails()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
        Exception innerException = new("Provider failure");

        IEventProviderService eventProviderService = CreateEventProviderService(
            new EventProvider<FakeObject>
            {
                Events = [inputName],
                SendHandler = (_, _, _) => ValueTask.FromException(innerException)
            });

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync(inputName, inputMessage);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(raiseEventAsyncTask);

        actualException.Should().BeSameAs(innerException);
    }
}
