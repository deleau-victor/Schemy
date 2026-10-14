using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Presentation.DependencyInjection;

internal static partial class PresentationDependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOpenApiDocumentation()
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer(TransformDocument);
                options.AddOperationTransformer(TransformOperation);
            });

            return services;
        }
    }

    private static Task TransformDocument(OpenApiDocument document, OpenApiDocumentTransformerContext _, CancellationToken __)
    {
        document.Info.Title = "Schemy API";
        document.Info.Version = "v1";
        document.Info.Description = "Stockage des schémas Mermaid et des croquis Excalidraw";

        ReplaceVersionPlaceholdersInPaths(document);
        MakeEnvelopeDataNonNullable(document);
        DeclareProblemDetailsExtensions(document);

        return Task.CompletedTask;
    }

    // ApiResponse<T>.Data is a required, non-null payload, but the generic parameter is unconstrained,
    // so the generated schema is "oneOf: [null, T]". Collapse it so clients see a non-nullable `data`.
    private static void MakeEnvelopeDataNonNullable(OpenApiDocument document)
    {
        if (document.Components?.Schemas is null)
        {
            return;
        }

        foreach ((string name, IOpenApiSchema schema) in document.Components.Schemas)
        {
            if (!name.StartsWith("ApiResponseOf", StringComparison.Ordinal)
                || schema.Properties is null
                || !schema.Properties.TryGetValue("data", out IOpenApiSchema? data)
                || data.OneOf is not { Count: 2 } oneOf)
            {
                continue;
            }

            IOpenApiSchema? payload = oneOf.FirstOrDefault(s => s.Type != JsonSchemaType.Null);
            if (payload is not null && oneOf.Any(s => s.Type == JsonSchemaType.Null))
            {
                schema.Properties["data"] = payload;
            }
        }
    }

    // ResultExtensions and GlobalExceptionHandler add `code`, `args`, `traceId` and `errors` as ProblemDetails
    // extension members, which the framework schema does not know about. Declare them for generated clients.
    private static void DeclareProblemDetailsExtensions(OpenApiDocument document)
    {
        if (document.Components?.Schemas is null
            || !document.Components.Schemas.TryGetValue("ProblemDetails", out IOpenApiSchema? schema)
            || schema is not OpenApiSchema problem)
        {
            return;
        }

        problem.Properties ??= new Dictionary<string, IOpenApiSchema>();

        var args = new OpenApiSchema { Type = JsonSchemaType.Array, Items = new OpenApiSchema() };

        problem.Properties["code"] = new OpenApiSchema { Type = JsonSchemaType.String };
        problem.Properties["args"] = args;
        problem.Properties["traceId"] = new OpenApiSchema { Type = JsonSchemaType.String };
        problem.Properties["errors"] = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            AdditionalProperties = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["code"] = new OpenApiSchema { Type = JsonSchemaType.String },
                        ["args"] = new OpenApiSchema { Type = JsonSchemaType.Array, Items = new OpenApiSchema() },
                    },
                    Required = new HashSet<string> { "code" },
                },
            },
        };

        problem.Required ??= new HashSet<string>();
        problem.Required.Add("code");
    }

    // Generated clients get concrete paths (/api/v1/...) instead of /api/v{version}/...
    private static void ReplaceVersionPlaceholdersInPaths(OpenApiDocument document)
    {
        if (document.Paths is null)
        {
            return;
        }

        var pathsToUpdate = document.Paths.ToList();
        document.Paths.Clear();

        foreach (KeyValuePair<string, IOpenApiPathItem> pathItem in pathsToUpdate)
        {
            string updatedPath = pathItem.Key
                .Replace("{version:apiVersion}", "1", StringComparison.Ordinal)
                .Replace("{version}", "1", StringComparison.Ordinal);
            document.Paths.Add(updatedPath, pathItem.Value);
        }
    }

    private static Task TransformOperation(OpenApiOperation operation, OpenApiOperationTransformerContext _, CancellationToken __)
    {
        IOpenApiParameter? versionParam = operation.Parameters?.FirstOrDefault(p => p.Name is "version");
        if (versionParam is not null)
        {
            operation.Parameters!.Remove(versionParam);
        }

        return Task.CompletedTask;
    }
}
