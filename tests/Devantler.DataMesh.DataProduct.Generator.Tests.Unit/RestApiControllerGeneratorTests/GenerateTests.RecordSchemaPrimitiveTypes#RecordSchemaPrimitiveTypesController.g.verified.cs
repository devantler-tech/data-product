﻿//HintName: RecordSchemaPrimitiveTypesController.g.cs
// <auto-generated>
// This code was generated by: 'Devantler.DataMesh.DataProduct.Generator.IncrementalGenerators.RestApiControllerGenerator'.
// Any changes made to this file will be overwritten.
using AutoMapper;
using Devantler.DataMesh.DataProduct.Models;
using Devantler.DataMesh.DataProduct.DataStore.Entities;
using Devantler.DataMesh.DataProduct.DataStore.Services;
namespace Devantler.DataMesh.DataProduct.Apis.Rest;
/// <summary>
/// A controller to handle REST API requests for a the <see cref="RecordSchemaPrimitiveTypes" /> model.
/// </summary>
public class RecordSchemaPrimitiveTypesController : RestApiController<RecordSchemaPrimitiveTypes, RecordSchemaPrimitiveTypesEntity>
{
    /// <summary>
    /// Creates a new instance of <see cref="RecordSchemaPrimitiveTypesController" />
    /// </summary>
    public RecordSchemaPrimitiveTypesController(DataStoreService<RecordSchemaPrimitiveTypes, RecordSchemaPrimitiveTypesEntity> dataStoreService) : base(dataStoreService)
    {
    }
}
