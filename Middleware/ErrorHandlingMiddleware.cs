using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace EmployeeManagement.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
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

        //private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        //{
        //    var statusCode = HttpStatusCode.InternalServerError; // Default to 500
        //    var response = new { message = "An error occurred while processing your request." };

        //    // Handle specific exception types
        //    if (exception is ArgumentNullException)
        //    {
        //        statusCode = HttpStatusCode.BadRequest;
        //        response = new { message = exception.Message };
        //    }
        //    else if (exception is UnauthorizedAccessException)
        //    {
        //        statusCode = HttpStatusCode.Unauthorized;
        //        response = new { message = "Unauthorized access." };
        //    }

        //    context.Response.ContentType = "application/json";
        //    context.Response.StatusCode = (int)statusCode;

        //    var json = JsonSerializer.Serialize(response);
        //    return context.Response.WriteAsync(json);
        //}

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError; // Default to 500
            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Status = (int)statusCode,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            if (exception is ArgumentNullException)
            {
                statusCode = HttpStatusCode.BadRequest;
                problemDetails.Status = (int)statusCode;
                problemDetails.Title = "Bad Request";
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                problemDetails.Status = (int)statusCode;
                problemDetails.Title = "Unauthorized";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(json);
        }

    }
}
