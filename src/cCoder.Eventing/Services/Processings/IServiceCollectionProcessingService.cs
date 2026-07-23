// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Services.Processings;

internal interface IServiceCollectionProcessingService
{
    void AddConfiguredEventingConfiguration(
        IServiceCollection services,
        Action<EventingConfiguration> newConfigure);

    void AddEventingConfiguration(
        IServiceCollection services,
        EventingConfiguration newEventingConfiguration);

    void AddEventProviders(
        IServiceCollection services,
        EventProvider[] newEventProviders);

    void AddBulkEventProviders(
        IServiceCollection services,
        BulkEventProvider[] newBulkEventProviders);

    void AddEventingForType<T>(IServiceCollection services);
}