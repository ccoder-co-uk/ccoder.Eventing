// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using cCoder.Eventing.Models;
using cCoder.Eventing.Sql.Models;

namespace cCoder.Eventing.Sql.Brokers;

internal sealed class SqlEventTransportBroker(SqlEventTransportConfiguration configuration)
    : ISqlEventTransportBroker
{
    public int GetMaxConcurrency() =>
        configuration.Options.MaxConcurrency;

    public TimeSpan GetPollInterval() =>
        configuration.Options.PollInterval;

    public TimeSpan GetLeaseDuration() =>
        configuration.Options.LeaseDuration;

    public string Serialize<T>(EventMessage<T> eventMessage) =>
        JsonSerializer.Serialize(
            value: new SqlEventEnvelope
            {
                SSOUserId = eventMessage.AuthInfo.SSOUserId,
                Data = JsonSerializer.SerializeToElement(
                    value: eventMessage.Data,
                    options: configuration.Options.JsonSerializerOptions)
            },
            options: configuration.Options.JsonSerializerOptions);

    public SqlEventEnvelope DeserializeEnvelope(string data) =>
        JsonSerializer.Deserialize<SqlEventEnvelope>(
            json: data,
            options: configuration.Options.JsonSerializerOptions);

    public EventProvider[] GetReceivers(string name) =>
        configuration.Providers
            .Where(predicate: provider => provider.CanReceive(name: name))
            .ToArray();

    public EventMessage CreateMessage(
        EventProvider eventProvider,
        SqlEventEnvelope sqlEventEnvelope)
    {
        object data = sqlEventEnvelope.Data.Deserialize(
            returnType: eventProvider.DataType,
            options: configuration.Options.JsonSerializerOptions);

        Type messageType = typeof(EventMessage<>).MakeGenericType(
            typeArguments: eventProvider.DataType);

        EventMessage eventMessage = (EventMessage)Activator.CreateInstance(type: messageType);

        eventMessage.AuthInfo = new EventAuthInfo
        {
            SSOUserId = sqlEventEnvelope.SSOUserId ?? "Guest"
        };

        messageType.GetProperty(name: "Data")
            .SetValue(obj: eventMessage, value: data);

        return eventMessage;
    }

    public Task ReceiveAsync(
        IServiceProvider scopedProvider,
        EventProvider eventProvider,
        string name,
        EventMessage eventMessage) =>
        eventProvider.ReceiveAsync(
            serviceProvider: scopedProvider,
            eventName: name,
            eventMessage: eventMessage)
        .AsTask();
}