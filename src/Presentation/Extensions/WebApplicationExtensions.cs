namespace Presentation.Extensions;

/// <summary>
/// Extensions for configuring the application middleware pipeline.
/// </summary>
internal static class WebApplicationExtensions
{
   extension(WebApplication app)
   {
      public async Task LaunchAsync()
      {
         ApiVersionSet versions = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

         app.UseExceptionHandler();

         app
            .UseOpenApiDocumentation()
            .MapHealthCheckEndpoints()
            .MapEndpoints(versions);

         await app.RunAsync();
      }

      /// <summary>
      /// OpenAPI + Swagger UI everywhere except Production: the web client is generated from it.
      /// </summary>
      private WebApplication UseOpenApiDocumentation()
      {
         if (!app.Environment.IsProduction())
         {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
               options.SwaggerEndpoint("/openapi/v1.json", "Schemy API v1");
               options.RoutePrefix = "swagger";
            });
         }

         return app;
      }

      private WebApplication MapHealthCheckEndpoints()
      {
         app.MapHealthChecks("/health");
         app.MapHealthChecks("/alive", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
         {
            Predicate = r => r.Tags.Contains("live")
         });

         return app;
      }
   }
}
