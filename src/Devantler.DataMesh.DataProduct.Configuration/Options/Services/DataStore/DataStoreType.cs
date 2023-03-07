namespace Devantler.DataMesh.DataProduct.Configuration.Options.Services.DataStore;

/// <summary>
/// Supported data store types.
/// </summary>
public enum DataStoreType
{
    /// <summary>
    /// A SQL data store.
    /// </summary>
    SQL,

    /// <summary>
    /// A NoSQL data store.
    /// </summary>
    NoSQL,
    /// <summary>
    /// A graph-based data store.
    /// </summary>
    Graph = 2
}
