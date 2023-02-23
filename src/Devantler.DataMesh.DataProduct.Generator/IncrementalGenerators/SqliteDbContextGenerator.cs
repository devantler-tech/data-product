using System.Collections.Immutable;
using System.Text;
using Chr.Avro.Abstract;
using Devantler.Commons.CodeGen.Core.Model;
using Devantler.Commons.CodeGen.CSharp;
using Devantler.Commons.CodeGen.CSharp.Model;
using Devantler.Commons.CodeGen.Mapping.Avro.Extensions;
using Devantler.Commons.StringHelpers;
using Devantler.DataMesh.DataProduct.Configuration.Options;
using Devantler.DataMesh.DataProduct.Configuration.Options.DataStoreOptions;
using Devantler.DataMesh.DataProduct.Configuration.Options.DataStoreOptions.Relational;
using Devantler.DataMesh.DataProduct.Generator.Extensions;
using Devantler.DataMesh.DataProduct.Generator.Models;
using Devantler.DataMesh.SchemaRegistry;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Devantler.DataMesh.DataProduct.Generator.IncrementalGenerators;

/// <summary>
/// A generator that generates a Sqlite database context.
/// </summary>
[Generator]
public class SqliteDbContextGenerator : GeneratorBase
{
    /// <summary>
    /// A method to generate a Sqlite database context.
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="additionalFiles"></param>
    /// <param name="options"></param>
    public override Dictionary<string, string> Generate(
        Compilation compilation,
        ImmutableArray<AdditionalFile> additionalFiles,
        DataProductOptions options
    )
    {
        var sources = new Dictionary<string, string>();
        if (options.DataStoreOptions.Type != DataStoreType.Relational
            && options.DataStoreOptions is not RelationalDataStoreOptionsBase
            {
                Provider: RelationalDataStoreProvider.SQLite
            })
        {
            return sources;
        }

        var schemaRegistryService = options.GetSchemaRegistryService();
        var rootSchema = schemaRegistryService.GetSchema(options.Schema.Subject, options.Schema.Version);

        var codeCompilation = new CSharpCompilation();

        var @class = new CSharpClass("SqliteDbContext")
            .AddImport(new CSharpUsing("Microsoft.EntityFrameworkCore"))
            .SetNamespace(NamespaceResolver.ResolveForType(compilation.GlobalNamespace,
                "RelationalDataStoreStartupExtensions"))
            .SetDocBlock(new CSharpDocBlock("A Sqlite database context."))
            .SetBaseClass(new CSharpClass("DbContext"))
            .AddConstructor(new CSharpConstructor("SqliteDbContext")
                .SetDocBlock(new CSharpDocBlock("A constructor to construct a Sqlite database context."))
                .AddParameter(new CSharpConstructorParameter("DbContextOptions<SqliteDbContext>", "options")
                    .SetIsBaseParameter(true)
                )
            );
        var onModelCreatingMethod = new CSharpMethod("OnModelCreating")
            .SetDocBlock(new CSharpDocBlock("A method to configure the model."))
            .AddParameter(new CSharpParameter("ModelBuilder", "modelBuilder"))
            .SetVisibility(Visibility.Protected)
            .SetIsOverride(true);

        foreach (var schema in rootSchema.Flatten())
        {
            if (schema is not RecordSchema recordSchema)
                continue;

            string schemaName = recordSchema.Name.ToPascalCase();
            _ = @class.AddProperty(new CSharpProperty($"DbSet<{schemaName}Entity>", $"{schemaName.ToPlural()}")
                .SetDocBlock(new CSharpDocBlock($"A property to access the {schemaName.ToKebabCase()} table."))
                .SetValue($"Set<{schemaName}Entity>()")
                .SetIsExpressionBodiedMember(true)
            );
            _ = onModelCreatingMethod.AddStatement(
                $"_ = modelBuilder.Entity<{schemaName}Entity>().ToTable(\"{schemaName}\");");
        }

        _ = @class.AddMethod(onModelCreatingMethod);

        _ = codeCompilation.AddType(@class);

        var codeGenerator = new CSharpCodeGenerator();
        return codeGenerator.Generate(codeCompilation);
    }
}
