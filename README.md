# cCoder.Eventing

`cCoder.Eventing` provides lightweight event distribution for cCoder domain
applications.

## Local Configuration

Configuration binds directly into `EventingConfiguration`. HTTP settings use
the `Eventing__Http__...` environment-variable prefix and Azure Service Bus
settings use `Eventing__ServiceBus__...`. Leave provider secrets empty in
appsettings, define them as user-level or machine-level environment variables,
restart Visual Studio, and press F5. Runtime event-handler delegates are created
in `Program.cs` before the Eventing registration call because delegates cannot
be supplied by the configuration binder.

It contains three packages:

- `cCoder.Eventing` for in-process publish/subscribe through `IEventHub`.
- `cCoder.Eventing.Http` for HTTP event handoff between application hosts.
- `cCoder.Eventing.AzureServiceBus` for Azure Service Bus backed event delivery.

## Functionality

The core package provides a DI-backed event hub. Applications register typed
handlers for named events and raise `EventMessage<T>` payloads from their
orchestration layer.

The HTTP package adds a default event endpoint at:

- `POST /Api/Eventing`
- `POST /Api/Eventing/Http`

Consumers configure the remote hub URL and register receive handlers through
`IHttpEventHub`. This keeps application code on top of the eventing hubs while
the package owns dispatching and message transport.

The Azure Service Bus package provides the same eventing shape for queue/topic
based delivery.

## Demo Apps

The repository includes two small runnable apps:

- `src/Apps/Eventing.App1`
- `src/Apps/Eventing.App2`

Each app hosts a SignalR chat UI at `/tools/index.html`, a health endpoint at
`/Health`, and the default HTTP eventing endpoint. Sending a chat message raises
`chat_event` locally through `IEventHub` and remotely through `IHttpEventHub`.
Both local and remote handlers broadcast received messages to connected SignalR
clients.

Default local URLs:

- App1: `https://localhost:7161`
- App2: `https://localhost:7162`

Run both apps, open both `/tools/index.html` pages, and send a message from
either side to see local and remote event handling.

## Configuration

The demo apps use standard ASP.NET Core configuration:

```json
{
  "EventingChat": {
    "AppName": "Eventing.App1",
    "RemoteHubUrl": "https://localhost:7162/Api/Eventing/Http"
  }
}
```

Runtime configuration should be provided through `appsettings.json`,
environment-specific appsettings files, and `AddEnvironmentVariables()` as part
of normal application startup. Runtime code should depend on typed configuration
models rather than reading environment variables directly.

## Tests

The solution contains:

- Unit tests for core, HTTP, and Azure Service Bus eventing.
- Acceptance tests for core package behavior.
- Acceptance tests for `Eventing.App1` and `Eventing.App2` health/UI startup.
- Integration tests that start both apps, connect SignalR clients to both, send
  a `chat_event`, and assert both app clients receive the message.

Run everything with:

```powershell
dotnet build src/cCoder.Eventing.sln -c Release -m:1 /p:BuildInParallel=false /p:UseSharedCompilation=false
dotnet test src/cCoder.Eventing.sln -c Release --no-build -m:1 /p:BuildInParallel=false /p:UseSharedCompilation=false
```
