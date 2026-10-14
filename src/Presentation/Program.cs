using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Presentation.DependencyInjection;
using Presentation.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddInfrastructure()
    .AddApplication()
    .AddPresentation();

WebApplication app = builder.Build();

await app.LaunchAsync();
