// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus.Services.Processings;

internal interface IServiceCollectionProcessingService
{
    void AddConfiguredAzureServiceBusEventingConfiguration(
        IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> newConfigure);

    void AddAzureServiceBusEventingConnection(
        IServiceCollection services,
        string serviceBusConnectionString);
}