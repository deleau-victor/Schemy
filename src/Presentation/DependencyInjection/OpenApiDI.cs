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

        return Task.CompletedTask;
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
