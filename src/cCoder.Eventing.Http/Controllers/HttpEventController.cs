// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Security;
using System.Security.Authentication;

using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Models.Exceptions;
using cCoder.Eventing.Http.Services.Processings;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Eventing.Http.Controllers;

[ApiController]
[Route("Api/Eventing")]
[Route("Api/Eventing/Http")]
public class HttpEventController(
    IHttpEventProcessingService httpEventProcessingService,
    ILoggingBroker loggingBroker) : ControllerBase
{
    [HttpPost]
    public async ValueTask<IActionResult> Post(
        HttpEventMessage newHttpEventMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            await httpEventProcessingService.ReceiveHttpEventMessageAsync(
                httpEventMessage: newHttpEventMessage,
                cancellationToken: cancellationToken);

            loggingBroker.LogInformation(
                message: "The event request was accepted.");

            return Accepted();
        }
        catch (ServiceValidationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The event request is invalid.");

            return BadRequest(error: "The event request is invalid.");
        }
        catch (AuthenticationException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The event request was not authenticated.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityException exception)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "The event request was forbidden.");

            return StatusCode(
                statusCode: 403,
                value: "The event request is forbidden.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "The event request failed.");

            return StatusCode(
                statusCode: 500,
                value: "The event request failed.");
        }
    }
}