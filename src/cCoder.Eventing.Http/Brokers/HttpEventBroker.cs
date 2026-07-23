// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace cCoder.Eventing.Http.Brokers;

internal class HttpEventBroker(
        IHttpClientFactory httpClientFactory,
        HttpEventingOptions options,
        ILogger<HttpEventBroker> log)
            : IHttpEventBroker
{
    public async ValueTask SendAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateOptions(options:options);

            HttpClient httpClient = httpClientFactory.CreateClient(
name:                HttpEventingOptions.HttpClientName);

            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
requestUri:                options.HubUrl,
value:                message,
options:                options.JsonSerializerOptions,
cancellationToken:                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst sending HTTP event {EventName}: {Message}",
                message?.EventName,
                ex.Message);

            throw;
        }
    }

    private static void ValidateOptions(HttpEventingOptions options)
    {
        if (string.IsNullOrWhiteSpace(value:options?.HubUrl))
        {
            throw new InvalidOperationException(
                "You must provide an HTTP event hub URL before sending events.");
        }
    }
}