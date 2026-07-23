// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public class EventAuthInfo : IEventAuthInfo
{
    public string SSOUserId { get; set; }
}