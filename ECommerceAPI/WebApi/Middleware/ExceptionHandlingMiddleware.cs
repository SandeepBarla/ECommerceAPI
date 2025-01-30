using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ECommerceAPI.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // Proceed with the next middleware
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException ?? ex;
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred."); // Log the error

            var response = context.Response;
            response.ContentType = "application/json";

            object errorResponse; // ✅ Define error response object

            switch (exception)
            {
                case ValidationException validationException:  // 🔹 Handle FluentValidation errors
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new
                    {
                        message = "Validation failed",
                        errors = validationException.Errors
                            .Select(e => new { field = e.PropertyName, error = e.ErrorMessage }) // ✅ Properly format validation errors
                    };
                    break;

                case KeyNotFoundException _:  // 🔹 Handle 404 Not Found
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { message = exception.Message ?? "Resource not found" };
                    break;

                case UnauthorizedAccessException _: // 🔹 Handle 401 Unauthorized
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new { message = exception.Message ?? "Unauthorized access" };
                    break;
                
                case InvalidOperationException :
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { message = exception.Message ?? "Please check the input" };
                    break;

                default:  // 🔹 Handle 500 Internal Server Error (General Exception)
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new { message = "An unexpected error occurred. Please try again later." };
                    break;
            }

            var jsonResponse = JsonConvert.SerializeObject(errorResponse);
            await response.WriteAsync(jsonResponse);
        }
    }
}
