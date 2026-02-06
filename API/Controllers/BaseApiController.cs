using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        // We want the Mediator to be available in all the controllers that will inherit from this BaseApiController, so we define it here as a protected property. This way, any controller that inherits from BaseApiController can access the Mediator property to send requests and receive responses without having to inject it separately in each controller.
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>() ?? throw new InvalidOperationException("IMediator service is unavailable");
    }
}
