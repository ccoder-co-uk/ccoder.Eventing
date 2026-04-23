using cCoder.Eventing.Http.Models;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Http.Controllers;

[ApiController]
[Route("Api/Eventing/Http")]
public class HttpEventController(IHttpEventHub httpEventHub) : ControllerBase
{
    [HttpPost]
    public async ValueTask<IActionResult> Post(
        HttpEventMessage message,
        CancellationToken cancellationToken)
    {
        await httpEventHub.ReceiveEventAsync(message, cancellationToken);
        return Accepted();
    }
}
