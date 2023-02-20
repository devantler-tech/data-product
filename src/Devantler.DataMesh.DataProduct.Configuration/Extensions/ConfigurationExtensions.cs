using Devantler.DataMesh.DataProduct.Configuration.Options;
using Devantler.DataMesh.DataProduct.Configuration.Options.DataStoreOptions;
using Devantler.DataMesh.DataProduct.Configuration.Options.DataStoreOptions.DocumentBased;
using Devantler.DataMesh.DataProduct.Configuration.Options.DataStoreOptions.Relational;
using Devantler.DataMesh.DataProduct.Configuration.Options.SchemaRegistryOptions;
using Devantler.DataMesh.DataProduct.Configuration.Options.SchemaRegistryOptions.Providers;
using Microsoft.Extensions.Configuration;

namespace Devantler.DataMesh.DataProduct.Configuration.Extensions;

/// <summary>
/// Extensions for the <see cref="IConfiguration"/> interface to get the data product options.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets the data product options from the configuration.
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static DataProductOptions GetDataProductOptions(this IConfiguration configuration)
    {
        var dataProductOptions = configuration.GetSection(DataProductOptions.Key).Get<DataProductOptions>()
            ?? throw new InvalidOperationException($"The configuration section '{DataProductOptions.Key}' is missing.");

        if (configuration.GetSection(DataStoreOptionsBase.Key).Exists())
            dataProductOptions.DataStoreOptions = SetDataStoreOptions(configuration);
        dataProductOptions.SchemaRegistryOptions.SetSchemaRegistryOptions(configuration);

        return dataProductOptions;
    }

    /// <summary>
    /// Gets the data store options from the configuration.
    /// </summary>
    /// <param name="configuration"></param>
    public static IDataStoreOptions SetDataStoreOptions(IConfiguration configuration)
    {
        var dataStoreType = configuration.GetSection(DataStoreOptionsBase.Key).GetValue<DataStoreType>("Type");

        string dataStoreProvider = dataStoreType switch
        {
            DataStoreType.Relational => configuration.GetSection(DataStoreOptionsBase.Key).GetValue<RelationalDataStoreProvider>("Provider").ToString(),
            DataStoreType.DocumentBased => configuration.GetSection(DataStoreOptionsBase.Key).GetValue<DocumentBasedDataStoreProvider>("Provider").ToString(),
            _ => throw new NotImplementedException($"The data store type '{dataStoreType}' is not implemented yet.")
        };

        return (dataStoreType, dataStoreProvider) switch
        {
            (DataStoreType.Relational, nameof(RelationalDataStoreProvider.SQLite)) => configuration.GetSection(DataStoreOptionsBase.Key).Get<SqliteDataStoreOptions>()
                ?? throw new InvalidOperationException($"Failed to bind the configuration instance '{nameof(SqliteDataStoreOptions)}' to the configuration section '{DataStoreOptionsBase.Key}"),
            (DataStoreType.DocumentBased, nameof(DocumentBasedDataStoreProvider.MongoDb)) => configuration.GetSection(DataStoreOptionsBase.Key).Get<MongoDbDataStoreOptions>()
                ?? throw new InvalidOperationException($"Failed to bind the configuration instance '{nameof(MongoDbDataStoreOptions)}' to the configuration section '{DataStoreOptionsBase.Key}"),
            _ => throw new NotImplementedException($"The combination of the data store type '{dataStoreType}' and the data store provider '{dataStoreProvider}' is not implemented yet.")
        };
    }

    /// <summary>
    /// Gets the schema registry options from the configuration.
    /// </summary>
    /// <param name="schemaRegistryOptions"></param>
    /// <param name="configuration"></param>
    public static void SetSchemaRegistryOptions(this ISchemaRegistryOptions schemaRegistryOptions, IConfiguration configuration)
    {
        if (!configuration.GetSection(SchemaRegistryOptionsBase.Key).Exists())
            return;

        var schemaRegistryType = configuration.GetSection(SchemaRegistryOptionsBase.Key).GetValue<SchemaRegistryType>("Type");

        schemaRegistryOptions = schemaRegistryType switch
        {
            SchemaRegistryType.Local => configuration.GetSection(SchemaRegistryOptionsBase.Key).Get<LocalSchemaRegistryOptions>()
                ?? throw new InvalidOperationException($"Failed to bind the configuration instance '{nameof(LocalSchemaRegistryOptions)}' to the configuration section '{SchemaRegistryOptionsBase.Key}"),
            SchemaRegistryType.Kafka => configuration.GetSection(SchemaRegistryOptionsBase.Key).Get<KafkaSchemaRegistryOptions>()
                ?? throw new InvalidOperationException($"Failed to bind the configuration instance '{nameof(KafkaSchemaRegistryOptions)}' to the configuration section '{SchemaRegistryOptionsBase.Key}"),
            _ => throw new NotImplementedException($"The schema registry type '{schemaRegistryType}' is not implemented yet.")
        };
    }
}
