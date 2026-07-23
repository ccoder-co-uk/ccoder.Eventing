// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Apps.Controllers;

[ApiController]
[Route("Api/Chat")]
public class ChatController(IChatOrchestrationService chatOrchestrationService) : ControllerBase
{
    [HttpPost]
    public async ValueTask<IActionResult> Post(
        ChatMessageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ChatMessage message =
                await chatOrchestrationService.SendAsync(request, cancellationToken);

            return Accepted(message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}