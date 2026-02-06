using System;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command : IRequest<string>
        {
            // The Activity property represents the activity that we want to create. It is marked as required, indicating that it must be provided when creating a new Command instance.
            public required Activity Activity { get; set; }
        }

        public class Handler(AppDbContext context) : IRequestHandler<Command, string>
        {
            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                // Add the new activity to the database context and save the changes asynchronously. Finally, return the Id of the newly created activity.
                // We are not using the async version of Add since it is not required here as we are not doing any I/O operation.
                context.Activities.Add(request.Activity);
                await context.SaveChangesAsync(cancellationToken);
                // Although the Id is already present in the Activity object since it is generated when the object is created and also we specified that commands should not return values as oppose to queries, we return it here to confirm the creation of the activity. We also want to return the Id so the client can use it if needed without having to generate it.
                return request.Activity.Id;
            }
        }
    }
}
