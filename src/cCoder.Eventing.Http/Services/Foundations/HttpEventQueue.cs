// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using System.Threading.Channels;

namespace cCoder.Eventing.Http.Services.Foundations;

internal class HttpEventQueue : IHttpEventQueue
{
    private readonly Channel<HttpEventMessage> channel =
        Channel.CreateUnbounded<HttpEventMessage>(
            new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

    public ValueTask EnqueueAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default) =>
            channel.Writer.WriteAsync(message, cancellationToken);

    public IAsyncEnumerable<HttpEventMessage> ReadAllAsync(
        CancellationToken cancellationToken = default) =>
            channel.Reader.ReadAllAsync(cancellationToken);
}