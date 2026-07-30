// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing;

public interface IAuthenticatedEventHub : IEventHub
{
    string CurrentUserId { get; }
}
