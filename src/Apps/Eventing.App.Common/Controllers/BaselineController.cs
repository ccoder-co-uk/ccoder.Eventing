using cCoder.Eventing.Apps.Exposures.Setup;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Apps.Controllers;

[ApiController]
[Route("Api/Eventing/Baseline")]
public sealed class BaselineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(EventingBaselinePackages.All);
}
