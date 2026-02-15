using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Result<ActivityDto>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<ActivityDto>>
    {
        // The purpose of the cancellation token is to allow the operation to be cancelled if it takes too long or if the client disconnects. By passing the cancellation token to the FindAsync method, we can ensure that if the operation is cancelled, it will stop executing and free up resources. This is especially important for long-running operations that may consume a lot of resources or for operations that may be affected by network issues or client disconnections. By using a cancellation token, we can improve the responsiveness and reliability of our application. If the user cancels the request or if the request takes too long, the cancellation token will signal the operation to stop, allowing our application to handle the cancellation gracefully and avoid unnecessary resource consumption. For example - if the user exits the page while the request is still processing, the cancellation token will signal the operation to stop, preventing it from continuing to run in the background and consuming resources unnecessarily.
        // The Handle method is responsible for processing the Query request. It uses the FindAsync method of the database context to retrieve the activity with the specified Id from the database. If the activity is not found, it throws an exception with a message indicating that the activity was not found. If the activity is found, it returns a successful Result object containing the activity. By using a Result type, we can provide a standardized way of representing both successful and failed outcomes of operations, allowing us to handle errors more effectively and provide clear feedback to clients.
        public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Note that the FindAsync method does not return related entities such as attendees or comments. If we need to include related entities, we would need to use a different method such as Include or ThenInclude to specify the related entities to be loaded along with the activity. However, in this case, we are only interested in retrieving the activity itself, so using FindAsync is sufficient and more efficient for our needs.
            // var activity = await context.Activities.FindAsync([request.Id], cancellationToken);

            // EAGER LOADING
            // The following is the eager approach which means that the related data is loaded from the database as part of the initial query. Other 2 options are Explicit loading which means that the related data is explicitly loaded from the database at a later time, and Lazy loading which means that the related data is transparently loaded from the database when the navigation property (the related data) is accessed (not so good in terms of performance). Another approach is projection
            /*
            var activity = await context.Activities
                .Include(x => x.Attendees) // This will include the ActivityAttendee data related to the selected activity
                .ThenInclude(x => x.User) // This will include the users that are attendees of the selected activity
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            */
            // EXLICIT LOADING
            // Implementing the Exlpicit loading approach
            // The ProjectTo method is used to project the query results directly into the specified destination type (ActivityDto in this case) using AutoMapper's mapping configuration. This allows us to retrieve only the necessary data from the database and map it directly to our DTO, improving performance by avoiding unnecessary loading of related entities. By using ProjectTo, we can efficiently retrieve and transform data in a single query, reducing the amount of data transferred from the database and improving the overall performance of our application.
            // The mapper.ConfigurationProvider refers to the configuration that AutoMapper uses to determine how to map objects.
            // Since we are doing the mapping here in the query, we can directly project the result to the ActivityDto without having to first retrieve the Activity entity and then map it to the DTO. This allows us to optimize the query and only retrieve the necessary data for the ActivityDto, improving performance by reducing the amount of data loaded from the database.
            // When we are using Explicit loading, auto mapper automatically uses Select to select the data needed for the activityDto object and then populates it
            var activity = await context.Activities.ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() }).FirstOrDefaultAsync(x => request.Id == x.Id, cancellationToken);

            if (activity == null)
            {
                // Instead of throwing an exception when the activity is not found, we return a failed Result object with an appropriate error message and status code. This allows us to handle the case where the activity is not found gracefully and provide a clear response to the client without relying on exceptions for control flow.
                return Result<ActivityDto>.Failure("Activity not found", 404);
            }
            // If the activity is found, we return a successful Result object containing the activity. This allows us to provide a standardized response format for both successful and failed outcomes, making it easier for clients to handle responses consistently.
            // return Result<ActivityDto>.Success(mapper.Map<ActivityDto>(activity));
            // Can directly return the activity since we already projected it to the ActivityDto in the query using AutoMapper's ProjectTo method, which means that the activity variable is already of type ActivityDto and we can return it directly without needing to map it again.
            return Result<ActivityDto>.Success(activity);
        }
    }
}
