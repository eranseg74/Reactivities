using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Queries
{
    public class GetActivityDetails
    {
        public class Query : IRequest<Result<Activity>>
        {
            public required string Id { get; set; }
        }

        public class Handler(AppDbContext context) : IRequestHandler<Query, Result<Activity>>
        {
            // The purpose of the cancellation token is to allow the operation to be cancelled if it takes too long or if the client disconnects. By passing the cancellation token to the FindAsync method, we can ensure that if the operation is cancelled, it will stop executing and free up resources. This is especially important for long-running operations that may consume a lot of resources or for operations that may be affected by network issues or client disconnections. By using a cancellation token, we can improve the responsiveness and reliability of our application. If the user cancels the request or if the request takes too long, the cancellation token will signal the operation to stop, allowing our application to handle the cancellation gracefully and avoid unnecessary resource consumption. For example - if the user exits the page while the request is still processing, the cancellation token will signal the operation to stop, preventing it from continuing to run in the background and consuming resources unnecessarily.
            // The Handle method is responsible for processing the Query request. It uses the FindAsync method of the database context to retrieve the activity with the specified Id from the database. If the activity is not found, it throws an exception with a message indicating that the activity was not found. If the activity is found, it returns a successful Result object containing the activity. By using a Result type, we can provide a standardized way of representing both successful and failed outcomes of operations, allowing us to handle errors more effectively and provide clear feedback to clients.
            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync([request.Id], cancellationToken);
                if (activity == null)
                {
                    // Instead of throwing an exception when the activity is not found, we return a failed Result object with an appropriate error message and status code. This allows us to handle the case where the activity is not found gracefully and provide a clear response to the client without relying on exceptions for control flow.
                    return Result<Activity>.Failure("Activity not found", 404);
                }
                // If the activity is found, we return a successful Result object containing the activity. This allows us to provide a standardized response format for both successful and failed outcomes, making it easier for clients to handle responses consistently.
                return Result<Activity>.Success(activity);
            }
        }
    }
}
