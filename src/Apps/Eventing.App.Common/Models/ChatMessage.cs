// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public class ChatMessage
{
    public Guid Id { get; set; }

    public string? SourceApp { get; set; }

    public string? User { get; set; }

    public string? Text { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
}