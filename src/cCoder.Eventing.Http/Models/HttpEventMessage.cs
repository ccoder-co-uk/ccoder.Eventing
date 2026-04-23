namespace cCoder.Eventing.Http.Models;

public class HttpEventMessage
{
    public string EventName { get; set; }

    public string SSOUserId { get; set; }

    public string Data { get; set; }
}
