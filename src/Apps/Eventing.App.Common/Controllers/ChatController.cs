// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Exposures;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Http.Brokers.Loggings;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Apps.Controllers;

[ApiController]
[Route("Api/Chat")]
public class ChatController(
    IChatManager chatOrchestrationService,
    EventingAppCommonConfiguration configuration,
    ILoggingBroker loggingBroker)
    : ControllerBase
{
    [HttpPost]
    public async ValueTask<IActionResult> Post(
        ChatMessageRequest newRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState:ModelState);
            }

            ChatMessage message =
                await chatOrchestrationService.SendChatMessageAsync(
                    newChatMessage: new ChatMessage
                    {
                        User = newRequest.User,
                        Text = newRequest.Text,
                        SourceApp = configuration.Eventing.AppName,
                    },
                    cancellationToken:cancellationToken);

            return Accepted(value:message);
        }
        catch (InvalidOperationException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "The chat request is invalid.");

            return BadRequest(error: "The chat request is invalid.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "The chat request failed.");

            return StatusCode(statusCode: 500);
        }
    }
}