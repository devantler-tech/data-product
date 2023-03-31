using Devantler.DataMesh.DataProduct.Configuration.Options;
using Devantler.DataMesh.DataProduct.Configuration.Options.CacheStore;
using Devantler.DataMesh.DataProduct.Configuration.Options.DataCatalog;
using Devantler.DataMesh.DataProduct.Configuration.Options.DataIngestors;
using Devantler.DataMesh.DataProduct.Configuration.Options.MetricsExporter;
using Devantler.DataMesh.DataProduct.Configuration.Options.SchemaRegistry;
using Devantler.DataMesh.DataProduct.Configuration.Options.SchemaRegistry.Providers;
using Devantler.DataMesh.DataProduct.Configuration.Options.TracingExporter;
using Microsoft.Extensions.Configuration;

namespace Devantler.DataMesh.DataProduct.Configuration;

/// <summary>
/// Extension methods for <see cref="IConfiguration"/>.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets the data product options.
    /// </summary>
    public static DataProductOptions GetDataProductOptions(this IConfiguration configuration)
    {
        var dataProductOptions = configuration.Get<DataProductOptions>()
            ?? throw new InvalidOperationException(
                $"Failed to bind configuration to the type '{typeof(DataProductOptions).FullName}'."
            );

        ConfigureCacheStoreOptions(configuration, dataProductOptions);
        ConfigureDataCatalogOptions(configuration, dataProductOptions);
        ConfigureDataIngestorsOptions(configuration, dataProductOptions);
        ConfigureMetricsExporterOptions(configuration, dataProductOptions);
        ConfigureTracingExporterOptions(configuration, dataProductOptions);
        ConfigureSchemaRegistryOptions(configuration, dataProductOptions);

        return dataProductOptions;
    }

    static void ConfigureCacheStoreOptions(IConfiguration configuration, DataProductOptions dataProductOptions)
    {
        if (dataProductOptions.FeatureFlags.EnableCaching)
        {
            dataProductOptions.CacheStore = dataProductOptions.CacheStore.Type switch
            {
                CacheStoreType.InMemory => configuration.GetSection(CacheStoreOptions.Key)
                    .Get<InMemoryCacheStoreOptions>()
                        ?? throw new InvalidOperationException(
                            $"Failed to bind configuration section '{CacheStoreOptions.Key}' to the type '{typeof(InMemoryCacheStoreOptions).FullName}'."
                        ),
                CacheStoreType.Redis => configuration.GetSection(CacheStoreOptions.Key)
                    .Get<RedisCacheStoreOptions>()
                        ?? throw new InvalidOperationException(
                            $"Failed to bind configuration section '{CacheStoreOptions.Key}' to the type '{typeof(RedisCacheStoreOptions).FullName}'."
                        ),
                _ => throw new NotSupportedException($"Cache store type '{dataProductOptions.CacheStore.Type}' is not supported.")
            };
        }
    }

    static void ConfigureDataCatalogOptions(IConfiguration configuration, DataProductOptions dataProductOptions)
    {
        if (dataProductOptions.FeatureFlags.EnableDataCatalog)
        {
            if (dataProductOptions.DataCatalog == null)
                throw new InvalidOperationException($"The configuration section '{DataCatalogOptions.Key}' is invalid or missing.");

            dataProductOptions.DataCatalog = dataProductOptions.DataCatalog.Type switch
            {
                DataCatalogType.DataHub => configuration.GetSection(DataCatalogOptions.Key)
                    .Get<DataHubDataCatalogOptions>()
                        ?? throw new InvalidOperationException(
                            $"Failed to bind configuration section '{DataCatalogOptions.Key}' to the type '{typeof(DataHubDataCatalogOptions).FullName}'."
                        ),
                _ => throw new NotSupportedException($"Data catalog type '{dataProductOptions.DataCatalog.Type}' is not supported.")
            };
        }
    }

    static void ConfigureDataIngestorsOptions(IConfiguration configuration, DataProductOptions dataProductOptions)
    {
        if (dataProductOptions.FeatureFlags.EnableDataIngestion)
        {
            var dataIngestors = new List<DataIngestorOptions>();
            var localDataIngestorOptions = configuration.GetSection(DataIngestorOptions.Key)
                .Get<List<LocalDataIngestorOptions>>()
                .Where(x => x.Type == DataIngestorType.Local);

            dataIngestors.AddRange(
                localDataIngestorOptions
            );

            dataIngestors.AddRange(
                configuration.GetSection(DataIngestorOptions.Key)
                    .Get<List<KafkaDataIngestorOptions>>()
                    .Where(x => x.Type == DataIngestorType.Kafka)
            );
            dataProductOptions.DataIngestors = dataIngestors;
        }
    }

    static void ConfigureMetricsExporterOptions(IConfiguration configuration, DataProductOptions dataProductOptions)
    {
        if (dataProductOptions.FeatureFlags.EnableTracing)
        {
            dataProductOptions.MetricsExporter = dataProductOptions.MetricsExporter.Type switch
            {
                MetricsExporterType.OpenTelemetry => configuration.GetSection(MetricsExporterOptions.Key)
                    .Get<OpenTelemetryMetricsExporterOptions>()
                        ?? throw new InvalidOperationException(
                            $"Failed to bind configuration section '{MetricsExporterOptions.Key}' to the type '{typeof(OpenTelemetryMetricsExporterOptions).FullName}'."
                        ),
                MetricsExporterType.Console => configuration.GetSection(MetricsExporterOptions.Key)
                    .Get<ConsoleMetricsExporterOptions>()
                        ?? throw new InvalidOperationException(
                            $"Failed to bind configuration section '{MetricsExporterOptions.Key}' to the type '{typeof(ConsoleMetricsExporterOptions).FullName}'."
                        ),
                _ => throw new NotSupportedException($"Tracing system type '{dataProductOptions.TracingExporter.Type}' is not supported.")
            };
        }
    }

    static void ConfigureTracingExporterOptions(IConfiguration configuration, DataProductOptions dataProductOptions)
    {
        if (dataProductOptions.FeatureFlags.EnableTracing)
        {
            dataProductOptions.TracingExporter = dataProductOptions.TracingExporter.Type switch
            {
                TracingExporterType.OpenTelemetry => configuration.GetSection(TracingExporterOptions.Key)
                    .Get<OpenTelemetryTracingExporterOptions>()
                        ?? throw new InvalidOperationException(
                            $"Failed to bind configuration section '{TracingExporterOptions.Key}' to the type '{typeof(OpenTelemetryTracingExporterOptions).FullName}'."
                        ),
                TracingExporterType.Console => configuration.GetSection(TracingExporterOptions.Key)
                    .Get<ConsoleTracingExporterOptions>()
                        ?? throw new InvalidOperationException(
                            $"Failed to bind configuration section '{TracingExporterOptions.Key}' to the type '{typeof(ConsoleTracingExporterOptions).FullName}'."
                        ),
                _ => throw new NotSupportedException($"Tracing system type '{dataProductOptions.TracingExporter.Type}' is not supported.")
            };
        }
    }

    static void ConfigureSchemaRegistryOptions(IConfiguration configuration, DataProductOptions dataProductOptions)
    {
        dataProductOptions.SchemaRegistry = dataProductOptions.SchemaRegistry.Type switch
        {
            SchemaRegistryType.Kafka => configuration.GetSection(SchemaRegistryOptions.Key)
                .Get<KafkaSchemaRegistryOptions>()
                    ?? throw new InvalidOperationException(
                        $"Failed to bind configuration section '{SchemaRegistryOptions.Key}' to the type '{typeof(KafkaSchemaRegistryOptions).FullName}'."
                    ),
            SchemaRegistryType.Local => configuration.GetSection(SchemaRegistryOptions.Key)
                .Get<LocalSchemaRegistryOptions>()
                    ?? throw new InvalidOperationException(
                        $"Failed to bind configuration section '{SchemaRegistryOptions.Key}' to the type '{typeof(LocalSchemaRegistryOptions).FullName}'."
                    ),
            _ => throw new NotSupportedException($"Schema registry type '{dataProductOptions.SchemaRegistry.Type}' is not supported.")
        };
    }
}