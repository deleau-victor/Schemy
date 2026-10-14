using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTests.Abstractions;

public class DispatcherTests
{
    public sealed record PingQuery(string Text) : IQuery<string>;

    public sealed record OrphanQuery : IQuery<string>;

    public sealed class PingQueryHandler : IQueryHandler<PingQuery, string>
    {
        public Task<Result<string>> Handle(PingQuery request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success($"pong:{request.Text}"));
    }

    private sealed class RecordingBehavior(string name, List<string> journal) : IPipelineBehavior<Result<string>>
    {
        public async Task<Result<string>> Handle(object request, RequestHandlerDelegate<Result<string>> nextHandler, CancellationToken cancellationToken)
        {
            journal.Add($"{name}:avant");
            Result<string> response = await nextHandler();
            journal.Add($"{name}:après");
            return response;
        }
    }

    [Fact]
    public async Task Send_ShouldRunBehaviorsInRegistrationOrder_ThenHandler()
    {
        List<string> journal = [];
        ServiceProvider provider = new ServiceCollection()
            .AddScoped<IRequestHandler<PingQuery, Result<string>>, PingQueryHandler>()
            .AddSingleton<IPipelineBehavior<Result<string>>>(new RecordingBehavior("premier", journal))
            .AddSingleton<IPipelineBehavior<Result<string>>>(new RecordingBehavior("second", journal))
            .BuildServiceProvider();
        var dispatcher = new Dispatcher(provider);

        Result<string> result = await dispatcher.Send(new PingQuery("x"));

        result.Value.ShouldBe("pong:x");
        journal.ShouldBe(["premier:avant", "second:avant", "second:après", "premier:après"]);
    }

    [Fact]
    public async Task Send_WithoutRegisteredHandler_ShouldThrow()
    {
        var dispatcher = new Dispatcher(new ServiceCollection().BuildServiceProvider());

        await Should.ThrowAsync<InvalidOperationException>(() => dispatcher.Send(new OrphanQuery()));
    }
}
