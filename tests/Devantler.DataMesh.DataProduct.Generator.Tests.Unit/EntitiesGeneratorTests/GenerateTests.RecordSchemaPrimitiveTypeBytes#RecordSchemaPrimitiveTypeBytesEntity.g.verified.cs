﻿//HintName: RecordSchemaPrimitiveTypeBytesEntity.g.cs
// <auto-generated>
// This code was generated by: 'Devantler.DataMesh.DataProduct.Generator.IncrementalGenerators.EntitiesGenerator'.
// Any changes made to this file will be overwritten.
using Devantler.DataMesh.DataProduct.DataStore.Interfaces;
namespace Devantler.DataMesh.DataProduct.DataStore.Relational;
public class RecordSchemaPrimitiveTypeBytesEntity : IEntity
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    public Guid Id { get; set; }
    public byte[] BytesField { get; set; }
}
