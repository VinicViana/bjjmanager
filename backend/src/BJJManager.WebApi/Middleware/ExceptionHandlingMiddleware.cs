using BJJManager.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BJJManager.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ProblemDetails problemDetails;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ValidationProblemDetails(validationException.Errors)
                {
                    Status = statusCode,
                    Title = "One or more validation errors occurred."
                };
                break;
            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                problemDetails = new ProblemDetails { Status = statusCode, Title = notFoundException.Message };
                break;
            case UnauthorizedAccessException unauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                problemDetails = new ProblemDetails { Status = statusCode, Title = unauthorizedAccessException.Message };
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                problemDetails = new ProblemDetails { Status = statusCode, Title = "An unexpected error occurred." };
                break;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
