using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    // We want the Mediator to be available in all the controllers that will inherit from this BaseApiController, so we define it here as a protected property. This way, any controller that inherits from BaseApiController can access the Mediator property to send requests and receive responses without having to inject it separately in each controller.
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>() ?? throw new InvalidOperationException("IMediator service is unavailable");

    // Since we specified that our handlers will return a Result type, we need to handle that in our API controller to return the appropriate HTTP responses based on the success or failure of the operation. The HandleResult method takes a Result object as a parameter and checks its properties to determine the appropriate response. If the operation was not successful and the error code is 404, it returns a NotFound response. If the operation was successful and there is a value, it returns that value. Otherwise, it returns a BadRequest response with the error message. By centralizing this logic in a single method, we can ensure consistent handling of results across all controllers that inherit from BaseApiController. This allows us to easily manage the response formatting and error handling in our API controllers, making it easier to maintain and update the response logic in one place rather than having to duplicate it across multiple controllers.
    // The purpose of the HandleResult method is to provide a standardized way to handle the results returned by our handlers and convert them into appropriate HTTP responses. By using this method, we can ensure that all controllers that inherit from BaseApiController handle results in a consistent way, making it easier to manage response formatting and error handling across our API controllers.
    // If we. did not specify <T> in the ActionResult return type, we would not be able to return the value of the result directly, and we would have to manually create an ObjectResult (like `return Ok(result.value)`) or JsonResult to return the value, which would add unnecessary complexity to our controller actions. By using ActionResult<T>, we can take advantage of the built-in functionality of ASP.NET Core to automatically handle the serialization and formatting of the response based on the type of the value being returned, making our controller actions cleaner and more concise.
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (!result.IsSuccess && result.Code == 404)
        {
            return NotFound();
        }
        if (result.IsSuccess && result.Value != null)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }
}
