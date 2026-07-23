// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public class ChatMessage
{
    public Guid Id { get; set; }

    public string SourceApp { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }
}