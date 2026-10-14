using Application.Features.Diagnostics.Queries.GetSystemInfo;
using Application.Features.Diagnostics.Responses;
using Presentation.Abstractions;
using Presentation.Extensions;

namespace Presentation.Endpoints;

internal sealed class DiagnosticsEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app, ApiVersionSet versions)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v{version:apiVersion}/system")
            .WithApiVersionSet(versions)
            .WithTags("System");

        group.MapGet("/info", async (IDispatcher dispatcher, CancellationToken cancellationToken) =>
                (await dispatcher.Send(new GetSystemInfoQuery(), cancellationToken)).ToHttpResult())
            .WithName("GetSystemInfo")
            .Produces<ApiResponse<SystemInfoResponse>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
