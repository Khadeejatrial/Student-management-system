using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errorResponse = new
        {
            StatusCode = response.StatusCode,
            Message = "A bad request occurred. Please check your input.",
            TraceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(errorResponse);
        return response.WriteAsync(json);
    }
}
