// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Security;
using System.Security.Authentication;

using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Models.Exceptions;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Http.Brokers.Loggings;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Apps.Controllers;

[ApiController]
[Route("Api/Chat")]
public class ChatController(
    IChatOrchestrationService chatOrchestrationService,
    AppConfiguration configuration,
    ILoggingBroker loggingBroker)
    : ControllerBase
{
    [HttpPost]
    public async ValueTask<IActionResult> Post(
        ChatMessageRequest newChatMessageRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(
                    message: "The chat request is invalid.");

                return BadRequest(modelState: ModelState);
            }

            ChatMessage message =
                await chatOrchestrationService.SendChatMessageAsync(
                    chatMessage: new ChatMessage
                    {
                        User = newChatMessageRequest.User,
                        Text = newChatMessageRequest.Text,
                        SourceApp = configuration.EventingChat.AppName,
                    },
                    cancellationToken: cancellationToken);

            loggingBroker.LogInformation(
                message: "The chat request was accepted.");

            return Accepted(value: message);
        }
        catch (ChatServiceValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The chat request is invalid.");

            return BadRequest(error: "The chat request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The chat request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The chat request was forbidden.");

            return StatusCode(
                statusCode: 403,
                value: "The chat request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "The chat request failed.");

            return StatusCode(
                statusCode: 500,
                value: "The chat request failed.");
        }
    }
}