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
    public async Task ShouldRethrowOnRaiseEventsAsyncIfProviderFails()
    {
        // Given

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
eventProviders: [],
bulkEventProviders: [
                new BulkEventProvider<FakeObject>
                {
                    Events = [inputName],
                    Handler = (_, _) => ValueTask.FromException(exception:innerException)
                }
            ]);

        // When

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventsAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}