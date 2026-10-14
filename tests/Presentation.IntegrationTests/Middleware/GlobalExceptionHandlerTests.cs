using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Presentation.Middleware;
using System.Text.Json;

namespace Presentation.IntegrationTests.Middleware;

public class GlobalExceptionHandlerTests
{
    private static async Task<(int Status, JsonElement Body)> HandleAsync(string environment, Exception exception)
    {
        var handler = new GlobalExceptionHandler(
            NullLogger<GlobalExceptionHandler>.Instance,
            new HostingEnvironment { EnvironmentName = environment });
        var context = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        context.Response.Body = new MemoryStream();

        (await handler.TryHandleAsync(context, exception, CancellationToken.None)).ShouldBeTrue();

        context.Response.Body.Position = 0;
        using JsonDocument json = await JsonDocument.ParseAsync(context.Response.Body);
        return (context.Response.StatusCode, json.RootElement.Clone());
    }

    [Fact]
    public async Task Production_ShouldHideExceptionMessage()
    {
        (int status, JsonElement body) = await HandleAsync("Production", new InvalidCastException("SELECT * FROM secret_table"));

        status.ShouldBe(StatusCodes.Status500InternalServerError);
        body.GetProperty("code").GetString().ShouldBe("Server.InternalError");
        body.GetProperty("traceId").GetString().ShouldNotBeNullOrEmpty();
        body.GetProperty("detail").GetString()!.ShouldNotContain("secret_table");
    }

    [Fact]
    public async Task Development_ShouldExposeExceptionMessage()
    {
        (_, JsonElement body) = await HandleAsync("Development", new InvalidCastException("détail utile"));

        body.GetProperty("detail").GetString().ShouldBe("détail utile");
    }
}
