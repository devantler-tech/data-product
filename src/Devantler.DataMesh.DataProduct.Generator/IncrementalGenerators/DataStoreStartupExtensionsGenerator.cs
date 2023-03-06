using System.Collections.Immutable;
using Chr.Avro.Abstract;
using Devantler.Commons.CodeGen.Core.Model;
using Devantler.Commons.CodeGen.CSharp;
using Devantler.Commons.CodeGen.CSharp.Model;
using Devantler.Commons.CodeGen.Mapping.Avro;
using Devantler.Commons.StringHelpers;
using Devantler.DataMesh.DataProduct.Configuration.Options;
using Devantler.DataMesh.DataProduct.Configuration.Options.ServiceOptions.DataStoreOptions;
using Devantler.DataMesh.DataProduct.Configuration.Options.ServiceOptions.DataStoreOptions.Relational;
using Devantler.DataMesh.DataProduct.Generator.Models;
using Devantler.DataMesh.SchemaRegistry;
using Microsoft.CodeAnalysis;

namespace Devantler.DataMesh.DataProduct.Generator.IncrementalGenerators;

/// <summary>
/// A generator for service registrations and usages for a data store.
/// </summary>
[Generator]
public class DataStoreStartupExtensionsGenerator : GeneratorBase
{
    /// <summary>
    /// Generates service registrations and usages for a data store.
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="additionalFiles"></param>
    /// <param name="options"></param>
    public override Dictionary<string, string> Generate(
        Compilation compilation,
        ImmutableArray<AdditionalFile> additionalFiles,
        DataProductOptions options)
    {
        var schemaRegistryService = options.Services.SchemaRegistry.CreateSchemaRegistryService();
        var rootSchema = schemaRegistryService.GetSchema(options.Services.SchemaRegistry.Schema.Subject, options.Services.SchemaRegistry.Schema.Version);

        var codeCompilation = new CSharpCompilation();

        string codeNamespace = NamespaceResolver.ResolveForType(compilation.GlobalNamespace, "DataStoreStartupExtensions");
        var dataStoreOptionsNamespace = NamespaceResolver.ResolveForType(compilation.GlobalNamespace, "IDataStoreOptions");
        var @class = new CSharpClass("DataStoreStartupExtensions")
            .AddImport(new CSharpUsing(string.IsNullOrEmpty(dataStoreOptionsNamespace) ? "Devantler.DataMesh.DataProduct.Configuration.Options.DataStoreOptions" : dataStoreOptionsNamespace))
            .AddImport(new CSharpUsing(NamespaceResolver.ResolveForType(compilation.GlobalNamespace, "IDataStoreService")))
            .AddImport(new CSharpUsing(NamespaceResolver.ResolveForType(compilation.GlobalNamespace, "IRepository")))
            .AddImport(new CSharpUsing(NamespaceResolver.ResolveForType(compilation.GlobalNamespace, "IEntity")))
            .AddImport(new CSharpUsing(NamespaceResolver.ResolveForType(compilation.GlobalNamespace, "IModel")))
            .AddImport(new CSharpUsing("Microsoft.EntityFrameworkCore"))
            .SetDocBlock(new CSharpDocBlock("A class that contains extension methods for service registrations and usages for a data store."))
            .SetNamespace(codeNamespace)
            .SetIsStatic(true)
            .SetIsPartial(true);

        var servicesParameter = new CSharpParameter("IServiceCollection", "services");
        var optionsParameter = new CSharpParameter("IDataStoreOptions", "options");
        var addGeneratedServiceRegistrationsMethod = new CSharpMethod("AddGeneratedServiceRegistrations")
            .SetIsStatic(true)
            .SetIsPartial(true)
            .SetIsExtensionMethod(true)
            .SetDocBlock(new CSharpDocBlock("Adds generated service registrations for a data store."))
            .SetVisibility(Visibility.Private)
            .AddParameter(servicesParameter)
            .AddParameter(optionsParameter);

        var appParameter = new CSharpParameter("WebApplication", "app");
        var useGeneratedServiceRegistrations = new CSharpMethod("UseGeneratedServiceRegistrations")
            .SetIsStatic(true)
            .SetIsPartial(true)
            .SetIsExtensionMethod(true)
            .SetDocBlock(new CSharpDocBlock("Uses generated service registrations for a data store."))
            .SetVisibility(Visibility.Private)
            .AddParameter(appParameter)
            .AddParameter(optionsParameter);

        switch (options.Services.DataStore.Type)
        {
            case DataStoreType.Relational:
                if (options.Services.DataStore is not RelationalDataStoreOptionsBase dataStoreOptions)
                    throw new InvalidOperationException("Relational data store options are not set.");
                _ = addGeneratedServiceRegistrationsMethod.AddStatement($"_ = services.AddPooledDbContextFactory<{dataStoreOptions.Provider}DbContext>(dbOptions => dbOptions.UseLazyLoadingProxies().Use{dataStoreOptions.Provider}(options?.ConnectionString));");
                foreach (var schema in rootSchema.Flatten().FindAll(s => s is RecordSchema).Cast<RecordSchema>())
                {
                    string schemaName = schema.Name.ToPascalCase();
                    _ = addGeneratedServiceRegistrationsMethod
                        .AddStatement($"_ = services.AddScoped<IRepository<{schemaName}Entity>, {schemaName}Repository>();")
                        .AddStatement($"_ = services.AddScoped<IDataStoreService<{schemaName}>, {schemaName}DataStoreService>();");
                }
                _ = useGeneratedServiceRegistrations
                    .AddStatement(
                        /*lang=csharp,strict*/
                        $$"""
                        using var scope = app.Services.CreateScope();
                        var services = scope.ServiceProvider;
                        var dbContextFactory = services.GetRequiredService<IDbContextFactory<{{dataStoreOptions.Provider}}DbContext>>();
                        using var context = dbContextFactory.CreateDbContext();
                        _ = context.Database.EnsureCreated();
                        """);
                break;
            case DataStoreType.DocumentBased:
                throw new NotSupportedException("Document based data stores are not supported yet.");
            case DataStoreType.GraphBased:
                throw new NotSupportedException("Graph based data stores are not supported yet.");
            default:
                throw new NotSupportedException($"Data store type '{options.Services.DataStore.Type}' is not supported.");
        }

        _ = @class.AddMethod(addGeneratedServiceRegistrationsMethod);
        _ = @class.AddMethod(useGeneratedServiceRegistrations);

        _ = codeCompilation.AddType(@class);

        var codeGenerator = new CSharpCodeGenerator();
        return codeGenerator.Generate(codeCompilation);
    }
}
