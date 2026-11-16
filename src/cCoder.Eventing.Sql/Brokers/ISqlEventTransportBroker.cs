// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.CodeAnalysis.Exposures;
using cCoder.Eventing.Models;
using cCoder.Eventing.Sql.Models;

namespace cCoder.Eventing.Sql.Brokers;

internal interface ISqlEventTransportBroker : IUtilityBroker
{
    int GetMaxConcurrency();
    TimeSpan GetPollInterval();
    TimeSpan GetLeaseDuration();
    string Serialize<T>(EventMessage<T> eventMessage);
    SqlEventEnvelope DeserializeEnvelope(string data);
    EventProvider[] GetReceivers(string name);
    EventMessage CreateMessage(EventProvider eventProvider, SqlEventEnvelope sqlEventEnvelope);
    Task ReceiveAsync(IServiceProvider scopedProvider, EventProvider eventProvider, string name, EventMessage eventMessage);
}