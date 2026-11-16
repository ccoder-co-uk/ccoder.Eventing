// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Sql.Services.Foundations;

internal sealed partial class SqlEventService
{
    private static void ValidateFailedOnGet(int count) =>
        Validate(inputs: [count]);

    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateMessage<T>(string name, EventMessage<T> message)
    {
        if (string.IsNullOrWhiteSpace(value: name) || name.Length > 100)
        {
            throw new ArgumentException("Event name must contain 1 to 100 characters.", nameof(name));
        }

        if (message.Data is null || message.AuthInfo is null)
        {
            throw new ArgumentException("An event needs data and auth information.", nameof(message));
        }
    }
}