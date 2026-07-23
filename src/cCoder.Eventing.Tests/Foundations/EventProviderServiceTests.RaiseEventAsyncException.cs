// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Models.Exceptions;
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
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        Exception innerException = new("Provider failure");

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders: new EventProvider<FakeObject>
            {
                Events = [inputName],
                SendHandler = (_, _, _) => ValueTask.FromException(exception:innerException)
            });

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}