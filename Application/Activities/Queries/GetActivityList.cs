using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    // The Query class represents the request for getting a list of activities. It implements the IRequest interface from MediatR, which indicates that this class is a request that can be handled by a handler. The type parameter List<Activity> specifies the type of response that the handler will return when it processes this request.
    // If the Query class had any properties, they would represent the parameters of the request. For example, if we wanted to filter the activities by a certain criteria, we could add properties to the Query class to represent those criteria in the curly brackets. In this case, since we want to get a list of all activities without any filtering, the Query class does not have any properties.
    public class Query : IRequest<Result<PagedList<ActivityDto, DateTime?>>>
    {
        public required ActivityParams Params { get; set; }
    }

    // The Handler class is responsible for handling the Query request. It implements the IRequestHandler interface from MediatR, which indicates that this class can handle requests of type Query and return a response of type List<Activity>. The constructor of the Handler class takes an instance of AppDbContext as a parameter, which is injected by the dependency injection system. This allows the handler to access the database context and perform operations on the database. The Handle method is where the logic for processing the Query request is implemented. In this case, it retrieves the list of activities from the database using Entity Framework Core's ToListAsync method and returns it as a response.
    public class Handler(AppDbContext context/*, ILogger<GetActivityList> logger*/, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<PagedList<ActivityDto, DateTime?>>>
    {
        // The purpose of the cancellation token is to allow the operation to be cancelled if it takes too long or if the client disconnects. By passing the cancellation token to the ToListAsync method, we can ensure that if the operation is cancelled, it will stop executing and free up resources. This is especially important for long-running operations that may consume a lot of resources or for operations that may be affected by network issues or client disconnections. By using a cancellation token, we can improve the responsiveness and reliability of our application. If the user cancels the request or if the request takes too long, the cancellation token will signal the operation to stop, allowing our application to handle the cancellation gracefully and avoid unnecessary resource consumption.
        // For example - if the user exits the page while the request is still processing, the cancellation token will signal the operation to stop, preventing it from continuing to run in the background and consuming resources unnecessarily.
        public async Task<Result<PagedList<ActivityDto, DateTime?>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // FAKE DELAY FOR SHOWING THE EFFECT OF THE CANCELLATION TOKEN
            // try
            // {
            //     for (int i = 0; i < 10; i++)
            //     {
            //         cancellationToken.ThrowIfCancellationRequested();
            //         await Task.Delay(1000, cancellationToken);
            //         logger.LogInformation($"Task {i} has completed");
            //     }
            // }
            // catch (Exception)
            // {
            //     logger.LogInformation("Task was cancelled");
            // }

            // Pagination. Constructing the query. This way we can build a query and then execute it. Here we are initially setting the query variable as query on the activities table. ordered by the date field.
            var query = context.Activities.OrderBy(x => x.Date).Where(x => x.Date >= (request.Params.Cursor ?? request.Params.StartDate)).AsQueryable();

            if (!string.IsNullOrEmpty(request.Params.Filter))
            {
                query = request.Params.Filter switch
                {
                    "isGoing" => query.Where(x => x.Attendees.Any(a => a.UserId == userAccessor.GetUserId())),
                    "isHost" => query.Where(x => x.Attendees.Any(a => a.IsHost && a.UserId == userAccessor.GetUserId())),
                    _ => query
                };
            }
            var projectedActivities = query.ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() });

            // Running the query while using the Take function for pagination. We take the page size + 1 so the last item will be the cursor for the next page
            var activities = await projectedActivities.Take(request.Params.PageSize + 1).ToListAsync(cancellationToken);

            DateTime? nextCursor = null;
            if (activities.Count > request.Params.PageSize)
            { // Setting the cursor to ne the date field of the last element in the list and then removing the last element in the list
                nextCursor = activities.Last().Date;
                activities.RemoveAt(activities.Count - 1);
            }

            if (activities == null)
            {
                return Result<PagedList<ActivityDto, DateTime?>>.Failure("Failed to get activities", 400);
            }
            return Result<PagedList<ActivityDto, DateTime?>>.Success(
                new PagedList<ActivityDto, DateTime?>
                {
                    Items = activities,
                    NextCursor = nextCursor
                }
            );
        }
    }
}
