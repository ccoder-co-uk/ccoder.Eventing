using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Objects.Interfaces;
using FluentAssertions;
using Xunit;

namespace EventLibrary.Tests
{
    public partial class ConfigurationTests
    {
        [Fact]
        public async void HubsRaiseEventsSecurely()
        {
            var userId = "Test";

            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => userId);
            builder.Services.AddEventingForType<object>();

            object inputEventData = new();
            object expectedResult = "Test";
            // when
            var app = builder.Build();
            var hub = app.Services.GetService<IEventHub>();
            object actualResult = null;

            hub.ListenToEvent<object>("object_event", async (serviceProvider, eventData) =>
            {
                var eventAuthBroker = serviceProvider.GetService<IEventAuthorizationBroker>();
                var authInfo = eventAuthBroker.GetEventAuthInfo();
                actualResult = authInfo.SSOUserId;
                await ValueTask.CompletedTask;
            });

            var inputEventMessage = new EventMessage<object>
            {
                Data = inputEventData,
                AuthInfo = new EventAuthInfo { SSOUserId = userId }
            };

            await hub.RaiseEventAsync("object_event", inputEventMessage);

            // then 
            actualResult.Should().Be(expectedResult);
        }

        [Fact]
        public async void NewEventScopesRelyOnSSOUserIdCorrectly()
        {
            var userId = "Test";

            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => userId);
            builder.Services.AddEventingForType<object>();

            object inputEventData = new();
            object expectedResult = "Test";
            object expectedResult2 = "Guest2";
            // when
            var app = builder.Build();
            var hub = app.Services.GetService<IEventHub>();
            object actualResult = null;

            hub.ListenToEvent<object>("object_event", async (serviceProvider, eventData) =>
            {
                var eventAuthBroker = serviceProvider.GetService<IEventAuthorizationBroker>();
                var authInfo = eventAuthBroker.GetEventAuthInfo();
                actualResult = authInfo.SSOUserId;
                await ValueTask.CompletedTask;
            });

            var inputEventMessage = new EventMessage<object> 
            { 
                Data = inputEventData,
                AuthInfo = new EventAuthInfo { SSOUserId = userId }
            };

            await hub.RaiseEventAsync("object_event", inputEventMessage);

            // then 
            actualResult.Should().Be(expectedResult);

            // when
            userId = "Guest2";

            inputEventMessage = new EventMessage<object>
            {
                Data = inputEventData,
                AuthInfo = new EventAuthInfo { SSOUserId = userId }
            };

            await hub.RaiseEventAsync("object_event", inputEventMessage);

            // then 
            actualResult.Should().Be(expectedResult2);
        }

        [Fact]
        public async void HubsRaiseEventsPersistCorrectUserId()
        {
            var userId = "Guest";

            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => userId);
            builder.Services.AddEventingForType<object>();

            object inputEventData = new();
            string expectedResult = "Guest2";

            // when
            var app = builder.Build();
            var hub = app.Services.GetService<IEventHub>();
            string actualResult = null;

            hub.ListenToEvent<object>("object_event", async (serviceProvider, eventData) =>
            {
                var eventAuthBroker = serviceProvider.GetService<IEventAuthorizationBroker>();
                var authInfo = eventAuthBroker.GetEventAuthInfo();
                actualResult = authInfo.SSOUserId;
                await ValueTask.CompletedTask;
            });

            var inputEventMessage = new EventMessage<object>
            {
                Data = inputEventData,
                AuthInfo = new EventAuthInfo { SSOUserId = expectedResult }
            };

            userId = "Guest2";

            await hub.RaiseEventAsync("object_event", inputEventMessage);

            // then 
            actualResult.Should().Be(expectedResult);
        }

        [Fact]
        public async void AuthInfoIsComputedCorrectly()
        {
            var userId = "Guest";

            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => userId);
            builder.Services.AddEventingForType<object>();

            object inputEventData = new();
            object expectedResult = "Guest2";

            // when
            var app = builder.Build();
            var hub = app.Services.GetService<IEventHub>();
            object actualResult = null;

            hub.ListenToEvent<object>("object_event", async (serviceProvider, eventData) =>
            {
                var authInfo = serviceProvider.GetService<IEventAuthInfo>();
                actualResult = authInfo.SSOUserId;
                await ValueTask.CompletedTask;
            });

            userId = "Guest2";

            var inputEventMessage = new EventMessage<object>
            {
                Data = inputEventData,
                AuthInfo = new EventAuthInfo { SSOUserId = userId }
            };

            await hub.RaiseEventAsync("object_event", inputEventMessage);

            // then 
            actualResult.Should().Be(expectedResult);
        }

        [Fact]
        public async void ChainedEventsRetainCorrectAuthInfo()
        {
            var originalUserId = "Hello World";
            var userId = "Hello World";

            // given
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEventing(x => userId);
            builder.Services.AddEventingForType<object>();

            object inputEventData = new();

            // when
            var app = builder.Build();
            var hub = app.Services.GetService<IEventHub>();
            IEventAuthInfo event1Auth = null;
            IEventAuthInfo event2Auth = null;

            async ValueTask event1Handler(IServiceProvider serviceProvider, object eventData)
            {
                userId = "Guest3";
                event1Auth = serviceProvider.GetService<IEventAuthInfo>();
                event1Auth.SSOUserId.Should().BeEquivalentTo(originalUserId);

                var inputEventMessage = new EventMessage<object>
                {
                    Data = inputEventData,
                    AuthInfo = new EventAuthInfo { SSOUserId = event1Auth.SSOUserId }
                };

                await hub.RaiseEventAsync("object_event2", inputEventMessage);
            }

            async ValueTask event2Handler(IServiceProvider serviceProvider, object eventData)
            {
                userId = "Guest";
                event2Auth = serviceProvider.GetService<IEventAuthInfo>();
                event2Auth.SSOUserId.Should().BeEquivalentTo(originalUserId);
                await ValueTask.CompletedTask;
            }

            hub.ListenToEvent<object>("object_event1", event1Handler);
            hub.ListenToEvent<object>("object_event2", event2Handler);

            var inputEventMessage = new EventMessage<object>
            {
                Data = inputEventData,
                AuthInfo = new EventAuthInfo { SSOUserId = userId }
            };

            await hub.RaiseEventAsync("object_event1", inputEventMessage);

            // then 
            event1Auth.Should().BeEquivalentTo(event2Auth);
        }
    }
}