﻿//HintName: RecordSchemaPrimitiveTypesEntity.g.cs
// <auto-generated>
// This code was generated by: 'Devantler.DataMesh.DataProduct.Generator.IncrementalGenerators.EntitiesGenerator'.
// Any changes made to this file will be overwritten.
using Devantler.DataMesh.DataProduct.DataStore.Interfaces;
namespace Devantler.DataMesh.DataProduct.DataStore.Relational;
public class RecordSchemaPrimitiveTypesEntity : IEntity
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    public Guid Id { get; set; }
    public bool BooleanField { get; set; }
    public byte[] BytesField { get; set; }
    public double DoubleField { get; set; }
    public float FloatField { get; set; }
    public int IntField { get; set; }
    public long LongField { get; set; }
    #nullable enable
    public object? NullField { get; set; }
    #nullable disable
    public string StringField { get; set; }
}
