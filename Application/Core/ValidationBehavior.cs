using FluentValidation;
using MediatR;

namespace Application.Core;

// This class is a MediatR pipeline behavior that is used to validate incoming requests using FluentValidation. It implements the IPipelineBehavior interface, which allows it to be added to the MediatR pipeline and executed before the request reaches its handler. The generic parameters TRequest and TResponse represent the type of the request and the type of the response, respectively. The constructor takes an optional IValidator<TRequest> parameter, which is used to perform the validation. If a validator is provided, it will be used to validate the incoming request, and if the validation fails, a ValidationException will be thrown with the validation errors. If no validator is provided, the behavior will simply pass the request to the next behavior in the pipeline without performing any validation.
// The cancellationToken parameter is used to propagate cancellation requests from the caller to the handler, allowing for graceful cancellation of long-running operations. By adding this validation behavior to the MediatR pipeline, we can ensure that all incoming requests are validated before they are processed by their respective handlers, improving the overall robustness and reliability of the application.
public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validator == null)
        {
            return await next(cancellationToken);
        }
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        return await next(cancellationToken);
    }
}
