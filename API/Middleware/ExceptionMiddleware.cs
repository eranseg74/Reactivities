using System.Text.Json;
using Application.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment environment) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    // The HandleException method is responsible for handling general exceptions that occur during the processing of HTTP requests. It takes in the HttpContext and the Exception as parameters. The method first logs the exception using the provided logger, including the exception message and stack trace. Then, it sets the response content type to "application/json" and the status code to 500 (Internal Server Error). Next, it creates an AppException object to represent the error response, including the status code, error message, and optionally the stack trace if the application is running in a development environment. Finally, it serializes the AppException object to JSON and writes it to the response body, allowing the client to receive a structured error response with relevant information about the exception that occurred.
    private async Task HandleException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, ex.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = environment.IsDevelopment() ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace) : new AppException(context.Response.StatusCode, ex.Message, null);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }

    // This method is responsible for handling validation exceptions that occur during the processing of HTTP requests. It takes in the HttpContext and the ValidationException as parameters. The method first creates a dictionary to store the validation errors, where the key is the property name and the value is an array of error messages associated with that property. It then iterates through the errors in the exception and populates the dictionary accordingly. After that, it sets the response status code to 400 (Bad Request) and creates a ValidationProblemDetails object to provide more details about the validation errors. Finally, it writes the ValidationProblemDetails object as a JSON response to the client. By centralizing the handling of validation exceptions in this middleware, we can ensure that all validation errors are handled consistently and that the client receives a clear and informative response when validation fails.
    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        // Create a dictionary to store the validation errors, where the key is the property name and the value is an array of error messages associated with that property. This allows us to easily organize and access the validation errors based on the properties that failed validation.
        var validationErrors = new Dictionary<string, string[]>();
        if (ex.Errors is not null)
        {
            foreach (var error in ex.Errors)
            {
                // Check if the dictionary already contains an entry for the property name. If it does, we append the new error message to the existing array of error messages. If it does not, we create a new entry in the dictionary with the property name as the key and an array containing the error message as the value. This way, we can handle multiple validation errors for the same property and provide a comprehensive list of errors to the client.
                if (validationErrors.TryGetValue(error.PropertyName, out var existingErrors))
                {
                    // If the property name already exists in the dictionary, append the new error message to the existing array of error messages.
                    validationErrors[error.PropertyName] = [.. existingErrors, error.ErrorMessage];
                }
                else
                {
                    // If the property name does not exist in the dictionary, create a new entry with the property name as the key and an array containing the error message as the value.
                    validationErrors[error.PropertyName] = [error.ErrorMessage];
                }
            }
        }
        // Set the response status code to 400 (Bad Request) to indicate that the request was invalid due to validation errors.
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        // Create a ValidationProblemDetails object to provide more details about the validation errors. This object includes the status code, a title, a detail message, and the dictionary of validation errors. By using the ValidationProblemDetails class, we can provide a standardized format for conveying validation errors to the client, making it easier for clients to understand and handle the errors appropriately.
        var validationProblemDetails = new ValidationProblemDetails(validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = "One or more validation errors has occurred",
        };
        // Write the ValidationProblemDetails object as a JSON response to the client. This allows the client to receive a clear and informative response that includes all the validation errors that occurred during the processing of the request. By centralizing the handling of validation exceptions in this middleware, we can ensure that all validation errors are handled consistently and that the client receives a standardized response format for validation errors.
        await context.Response.WriteAsJsonAsync(validationProblemDetails);
    }
}
