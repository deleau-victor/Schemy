using Application.Features.Diagnostics.Responses;
using System.Reflection;

namespace Application.Features.Diagnostics.Queries.GetSystemInfo;

public sealed record GetSystemInfoQuery : IQuery<SystemInfoResponse>;

internal sealed class GetSystemInfoQueryHandler : IQueryHandler<GetSystemInfoQuery, SystemInfoResponse>
{
    private static readonly string Version =
        typeof(GetSystemInfoQuery).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? "0.0.0";

    public Task<Result<SystemInfoResponse>> Handle(GetSystemInfoQuery request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(new SystemInfoResponse("Schemy", Version)));
}
