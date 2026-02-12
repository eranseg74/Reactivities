using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class DeleteActivity
{

    // The Command class represents the request to delete an activity. It implements the IRequest interface from MediatR, which allows it to be sent through the MediatR pipeline and handled by a corresponding handler. The Command class contains a single property, Id, which is required and represents the unique identifier of the activity to be deleted. By using a Command class, we can encapsulate the data needed for the delete operation and provide a clear contract for the request, making it easier to manage and maintain our application's command handling logic. We use the Unit type from MediatR to indicate that this command does not return any data upon successful completion, as the primary purpose of this command is to perform an action (deleting an activity) rather than returning a result. However, we can still return a Result<Unit> to indicate the success or failure of the operation, allowing us to provide feedback to the client about the outcome of the delete operation.
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities.FindAsync([request.Id], cancellationToken: cancellationToken);

            if (activity == null)
            {
                return Result<Unit>.Failure("Activity not found", 404);
            }

            context.Activities.Remove(activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;
            if (!result)
            {
                return Result<Unit>.Failure("Failed to delete activity", 400);
            }
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
