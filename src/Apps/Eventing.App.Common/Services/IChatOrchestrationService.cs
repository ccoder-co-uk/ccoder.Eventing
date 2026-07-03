using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services;

public interface IChatOrchestrationService
{
    ValueTask<ChatMessage> SendAsync(
        ChatMessageRequest request,
        CancellationToken cancellationToken = default);

    ValueTask ReceiveAsync(ChatMessage message);
}
