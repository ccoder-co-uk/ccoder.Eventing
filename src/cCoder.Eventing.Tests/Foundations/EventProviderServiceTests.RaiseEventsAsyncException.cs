using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventProviderServiceTests
{
    [Fact]
    public async Task ShouldRethrowOnRaiseEventsAsyncIfProviderFails()
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
        Exception innerException = new("Provider failure");

        IEventProviderService eventProviderService = CreateEventProviderService(
            [],
            [
                new BulkEventProvider<FakeObject>
                {
                    Events = [inputName],
                    Handler = (_, _) => ValueTask.FromException(innerException)
                }
            ]);

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync(inputName, inputMessages);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(raiseEventsAsyncTask);

        actualException.Should().BeSameAs(innerException);
    }
}
