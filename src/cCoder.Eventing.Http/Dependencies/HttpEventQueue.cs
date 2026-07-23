// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using System.Threading.Channels;

namespace cCoder.Eventing.Http.Dependencies;

internal class HttpEventQueue : IHttpEventQueue
{
    private readonly Channel<HttpEventMessage> channel =
        Channel.CreateUnbounded<HttpEventMessage>(
options: new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

    public ValueTask EnqueueAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default) =>
        channel.Writer.WriteAsync(item:message, cancellationToken:cancellationToken);

    public IAsyncEnumerable<HttpEventMessage> ReadAllAsync(
        CancellationToken cancellationToken = default) =>
        channel.Reader.ReadAllAsync(cancellationToken:cancellationToken);
}