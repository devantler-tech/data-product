﻿//HintName: DataIngestionStartupExtensions.g.cs
// <auto-generated>
// This code was generated by: 'Devantler.DataMesh.DataProduct.Generator.IncrementalGenerators.DataIngestionStartupExtensionsGenerator'.
// Any changes made to this file will be overwritten.
using ;
using Devantler.DataMesh.DataProduct.Features.DataIngestion.Ingestors;
using Devantler.DataMesh.DataProduct.Schemas;
namespace Devantler.DataMesh.DataProduct.Features.DataIngestion;
/// <summary>
/// A class that contains extension methods for service registrations and usages for data ingestors
/// </summary>
public static partial class DataIngestionStartupExtensions
{
    /// <summary>
    /// Adds generated service registrations for data ingestors.
    /// </summary>
    static partial void AddGeneratedServiceRegistrations(this IServiceCollection services, List<IDataIngestorOptions> options)
    {
        _ = services.AddHostedService<LocalDataIngestor<RecordSchemaPrimitiveTypes>>();
    }
}