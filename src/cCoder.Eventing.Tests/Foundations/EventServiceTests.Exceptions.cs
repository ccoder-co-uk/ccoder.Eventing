// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.Eventing.Models;
using cCoder.Eventing.Models.Exceptions;
using cCoder.Eventing.Services.Foundations;
using FluentAssertions;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
{
    [Fact]
    public void ShouldThrowValidationExceptionOnListenToEventIfNameIsInvalid()
    {
        // Given

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        // When

        Action listenToEventAction = () =>
            eventService.ListenToEvent(name: null!, handler: inputHandler);

        // Then

        Assert.Throws<ServiceValidationException>(testCode: listenToEventAction);
    }

    [Fact]
    public void ShouldThrowServiceExceptionOnListenToEventIfHandlerCollectionFails()
    {
        // Given

        const string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        FieldInfo handlersByNameField =
            typeof(EventService<FakeObject>).GetField(
                name: "handlersByName",
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic)!;

        var handlersByName =
            (IDictionary<
                string,
                ICollection<Func<IServiceProvider, FakeObject, ValueTask>>>)
                    handlersByNameField.GetValue(obj: eventService)!;

        handlersByName[inputName] =
            Array.AsReadOnly(array: new[] { inputHandler });

        // When

        Action listenToEventAction = () =>
            eventService.ListenToEvent(name: inputName, handler: inputHandler);

        // Then

        ServiceException actualException =
            Assert.Throws<ServiceException>(testCode: listenToEventAction);

        actualException.InnerException.Should()
            .BeOfType<NotSupportedException>();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnRaiseEventAsyncIfMessageIsInvalid()
    {
        // Given

        const string inputName = "event-name";

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(
                name: inputName,
                message: null!);

        // Then

        await Assert.ThrowsAsync<ServiceValidationException>(
            testCode: raiseEventAsyncTask);
    }
}