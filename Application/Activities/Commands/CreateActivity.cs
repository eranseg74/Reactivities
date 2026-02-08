using System;
using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command : IRequest<Result<string>>
        {
            // The Activity property represents the activity that we want to create. It is marked as required, indicating that it must be provided when creating a new Command instance.
            public required CreateActivityDto ActivityDto { get; set; }
        }

        // We use the IValidator interface from FluentValidation to validate the incoming command before processing it. This ensures that the data we receive is valid and meets our defined rules. The IMapper interface from AutoMapper is used to map the CreateActivityDto to the Activity entity, which allows us to easily convert between the two types without having to manually copy properties. The AppDbContext is used to interact with the database and save the new activity.
        // In this approach we will be required to inject the IValidator and validate for each command and this might be too much. A better approach is to catch and handle all the validation errors in a middleware since we already defined it as a service in the Program.cs file.
        public class Handler(AppDbContext context, IMapper mapper/*, IValidator<Command> validator*/) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                // We use the ValidateAndThrowAsync method to validate the incoming command. If the validation fails, it will throw an exception that can be caught and handled by our API controller to return a proper error response to the client. By validating the command before processing it, we can ensure that we are working with valid data and prevent any potential issues that may arise from invalid input.
                // Removing the validation from the handler and adding it as a pipeline behavior in the MediatR pipeline allows us to centralize the validation logic and avoid having to inject and validate for each command separately. This way, we can ensure that all commands are validated consistently without having to repeat the validation code in each handler.
                // await validator.ValidateAndThrowAsync(request, cancellationToken);
                // When using the AutoMapper we need to define in the angle brackets (<>) the object we are going to map to and as a property, the object we are going to map from. This will produce an Activity object with all the fields defined in the CreateActivityDto which is the type we defined in the Command class above. When adding and saving to the DB the Id will be generated automatically and the IsCancelled (both do not exist in the DTO) will remain empty. After the activity is saved and has an Id, we return that Id.
                var activity = mapper.Map<Activity>(request.ActivityDto);
                // Add the new activity to the database context and save the changes asynchronously. Finally, return the Id of the newly created activity.
                // We are not using the async version of Add since it is not required here as we are not doing any I/O operation.
                context.Activities.Add(activity);
                var result = await context.SaveChangesAsync(cancellationToken) > 0;
                // Although the Id is already present in the Activity object since it is generated when the object is created and also we specified that commands should not return values as oppose to queries, we return it here to confirm the creation of the activity. We also want to return the Id so the client can use it if needed without having to generate it.

                if (!result)
                {
                    return Result<string>.Failure("Failed to save the new activity in the DB", 400);
                }

                return Result<string>.Success(activity.Id);
            }
        }
    }
}
