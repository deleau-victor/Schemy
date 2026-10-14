namespace Presentation.DependencyInjection;

internal static class PresentationDependencyInjectionRoot
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddPresentation()
        {
            builder.AddServiceDefaults();
            builder.Services
                .AddOpenApiDocumentation()
                .AddApiVersioning()
                .AddExceptionHandling()
                .ConfigureJsonOptions();
            return builder;
        }
    }
}
