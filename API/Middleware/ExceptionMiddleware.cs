using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DDD_ExceptionHandling.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (DomainException ex)
        {
            this._logger.LogError(ex, "A domain exception occurred: {ExMessage}", ex.Message);
            await HandleDomainExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "An unexpected error occurred: {ExMessage}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        var problemDetails = exception switch
        {
            DomainValidationException validationEx => new ValidationProblemDetails(validationEx.Errors)
            {
                Title = "Ungültige Anfrageparameter",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            BusinessRuleValidationException => new ValidationProblemDetails
            {
                Title = "Geschäftsregel verletzt",
                Status = StatusCodes.Status409Conflict,
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            _ => new ValidationProblemDetails
            {
                Title = "Domainfehler",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message,
                Instance = context.Request.Path
            }
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Ein unerwarteter Fehler ist aufgetreten",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}