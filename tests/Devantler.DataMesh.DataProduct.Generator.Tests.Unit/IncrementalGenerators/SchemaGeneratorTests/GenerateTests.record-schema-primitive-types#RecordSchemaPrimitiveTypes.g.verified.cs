﻿//HintName: RecordSchemaPrimitiveTypes.g.cs
// <auto-generated>
// This code was generated by: 'Devantler.DataMesh.DataProduct.Generator.IncrementalGenerators.SchemaGenerator'.
// Any changes made to this file will be overwritten.
namespace Devantler.DataMesh.DataProduct.Schemas;
/// <summary>
/// An schema class for the RecordSchemaPrimitiveTypes record.
/// </summary>
public class RecordSchemaPrimitiveTypes : ISchema
{
    /// <summary>
    /// The unique identifier for this schema.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// The BooleanField property.
    /// </summary>
    public bool BooleanField { get; set; }
    /// <summary>
    /// The BytesField property.
    /// </summary>
    public byte[] BytesField { get; set; }
    /// <summary>
    /// The DoubleField property.
    /// </summary>
    public double DoubleField { get; set; }
    /// <summary>
    /// The FloatField property.
    /// </summary>
    public float FloatField { get; set; }
    /// <summary>
    /// The IntField property.
    /// </summary>
    public int IntField { get; set; }
    /// <summary>
    /// The LongField property.
    /// </summary>
    public long LongField { get; set; }
    #nullable enable
    /// <summary>
    /// The NullField property.
    /// </summary>
    public string? NullField { get; set; }
    #nullable disable
    /// <summary>
    /// The StringField property.
    /// </summary>
    public string StringField { get; set; }
}
