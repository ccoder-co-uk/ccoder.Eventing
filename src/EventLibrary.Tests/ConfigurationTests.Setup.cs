using EventLibrary.Objects;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Xunit;

namespace EventLibrary.Tests
{
    public partial class ConfigurationTests
    {
        [Fact]
        public void HubsInitializeProperly()
        {
            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => "Guest");
            builder.Services.AddEventingForType<object>();

            // when
            var app = builder.Build();
            var hub1 = app.Services.GetService<IEventHub>();
            var hub2 = app.Services.GetService<IEventHub>();

            hub1.ListenToEvent<object>("object_event", HandleTestEvent);

            var hub1Services = hub1.GetType()
                .GetField("services", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(hub1)
                    as IList<object>;

            var hub2Services = hub2.GetType()
                .GetField("services", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(hub2)
                    as IList<object>;

            // then
            hub1.Should().Be(hub2);
        }

        [Fact]
        public void HubsListenToEvents()
        {
            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => "Guest");
            builder.Services.AddEventingForType<object>();

            // when
            var app = builder.Build();
            var hub = app.Services.GetService<IEventHub>();
            hub.ListenToEvent<object>("object_event", HandleTestEvent);

            var services = hub.GetType()
                .GetField("services", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(hub)
                    as IList<object>;

            // then
            services.Should().NotBeNull();
            services.Should().NotBeEmpty();
            services.Should().HaveCount(1);
        }

        [Fact]
        public async void HubsRaiseEvents()
        {
            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => "Guest");
            builder.Services.AddEventingForType<object>();

            object inputEventData = new { X = "y", z = 1 };
            object expectedResult = inputEventData;
            // when
            var app = builder.Build();
            var hub = app.Services.GetService<IEventHub>();
            object actualResult = null;

            hub.ListenToEvent<object>("object_event", async (serviceProvider, eventData) =>
            {
                actualResult = eventData;
                await ValueTask.CompletedTask;
            });

            var inputEventMessage = new EventMessage<object>
            {
                Data = inputEventData,
                AuthInfo = new EventAuthInfo { SSOUserId = "Guest" }
            };

            await hub.RaiseEventAsync("object_event", inputEventMessage);

            // then 
            actualResult.Should().Be(expectedResult);
        }
    }
}