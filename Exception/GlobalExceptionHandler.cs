using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcarePortalApi.Exception;

// 1. Inject the Logger via Primary Constructor
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        System.Exception exception, 
        CancellationToken cancellationToken)
    {
        // 2. Log the raw exception for your internal debugging
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        // 3. Determine the Status Code based on the Exception Type
        var statusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            DbUpdateException => StatusCodes.Status400BadRequest,
            ArgumentException or ArgumentNullException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        // 4. Create a safe, standardized error response for the frontend
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = statusCode == 500 
                ? "An unexpected system error occurred." // Hide 500 details from the user
                : exception.Message // Safe to show client mistakes
        };

        // 5. Write the response
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // 6. Return true to tell .NET this exception is officially handled
        return true;
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        _ => "Internal Server Error"
    };
}