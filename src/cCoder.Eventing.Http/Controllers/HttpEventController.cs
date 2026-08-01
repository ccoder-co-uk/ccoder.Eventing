// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Http.Controllers;

[ApiController]
[Route("Api/Eventing")]
[Route("Api/Eventing/Http")]
public class HttpEventController(IHttpEventHub httpEventHub) : ControllerBase
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
        catch (InvalidOperationException)
        {
            return BadRequest(error: "The event request is invalid.");
        }
        catch (Exception)
        {
            return StatusCode(statusCode: 500);
        }
    }
}