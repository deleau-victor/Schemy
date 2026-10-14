namespace Presentation.Middleware;

/// <summary>
/// Global exception handler that converts unhandled exceptions to ProblemDetails responses
/// using the same shape as <c>ResultExtensions.ToErrorResult</c> (code, args, traceId) so the
/// front-end has one parser for every error.
/// In Development the original exception message is included in <c>detail</c> for debugging;
/// in Production a constant message is returned to avoid leaking stack traces, SQL fragments
/// or filesystem paths.
/// </summary>
internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment env) : IExceptionHandler
{
   private readonly bool _isDevelopment = env.IsDevelopment();

   public async ValueTask<bool> TryHandleAsync(
       HttpContext httpContext,
       Exception exception,
       CancellationToken cancellationToken)
   {
      logger.LogError(exception, "Unhandled exception occurred");

      // Every unhandled exception is a server fault: exception types (EF Core, Npgsql, the dispatcher...)
      // say nothing about whether the client is at fault, so never map them to 4xx.
      const int statusCode = StatusCodes.Status500InternalServerError;
      const string title = "Internal Server Error";
      const string code = "Server.InternalError";

      string detail = _isDevelopment
          ? exception.Message
          : "An unexpected error occurred. Please try again later.";

      ProblemDetails problemDetails = new()
      {
         Status = statusCode,
         Title = title,
         Detail = detail,
         Type = $"https://httpstatuses.com/{statusCode}",
         Extensions =
         {
            ["code"] = code,
            ["args"] = Array.Empty<object>(),
            ["traceId"] = httpContext.TraceIdentifier,
         },
      };

      httpContext.Response.StatusCode = statusCode;
      httpContext.Response.ContentType = "application/problem+json";

      await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

      return true;
   }
}
