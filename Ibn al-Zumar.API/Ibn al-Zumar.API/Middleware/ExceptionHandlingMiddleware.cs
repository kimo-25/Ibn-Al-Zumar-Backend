// File: Middleware/ExceptionHandlingMiddleware.cs
using System.Text.Json;
using IbnAlZumar.Api.Common.Exceptions;
using IbnAlZumar.Api.DTOs.Common;

namespace IbnAlZumar.Api.Middleware;

/// <summary>
/// Catches every unhandled exception in the pipeline and converts it into a consistent
/// ApiErrorResponse JSON body instead of letting the raw exception (or a bare 500) leak out.
/// Register this FIRST in Program.cs so it wraps everything downstream, including auth.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new ApiErrorResponse { TraceId = context.TraceIdentifier };

        switch (exception)
        {
            case ValidationAppException validationEx:
                response.StatusCode = validationEx.StatusCode;
                response.Message = validationEx.Message;
                response.Errors = validationEx.Errors;
                _logger.LogWarning(exception, "Validation error: {Message}", exception.Message);
                break;

            case AppException appEx:
                response.StatusCode = appEx.StatusCode;
                response.Message = appEx.Message;
                _logger.LogWarning(exception, "Handled application exception: {Message}", exception.Message);
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                // Never leak internal exception details/stack traces to the client outside Development.
                response.Message = _env.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred. Please try again later.";
                _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}