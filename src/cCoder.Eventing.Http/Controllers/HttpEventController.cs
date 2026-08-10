// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Models;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Http.Controllers;

[ApiController]
[Route("Api/Eventing")]
[Route("Api/Eventing/Http")]
public class HttpEventController(
    IHttpEventHub httpEventHub,
    ILoggingBroker loggingBroker) : ControllerBase
{
    [HttpPost]
    public async ValueTask<IActionResult> Post(
        HttpEventMessage newMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            await httpEventHub.ReceiveEventAsync(
                message: newMessage,
                cancellationToken: cancellationToken);

            return Accepted();
        }
        catch (InvalidOperationException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "The event request is invalid.");

            return BadRequest(error: "The event request is invalid.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "The event request failed.");

            return StatusCode(statusCode: 500);
        }
    }
}