using Application.Activities.DTOs;
using Application.Core;
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
    public class Query : IRequest<Result<List<ActivityDto>>> { }

    // The Handler class is responsible for handling the Query request. It implements the IRequestHandler interface from MediatR, which indicates that this class can handle requests of type Query and return a response of type List<Activity>. The constructor of the Handler class takes an instance of AppDbContext as a parameter, which is injected by the dependency injection system. This allows the handler to access the database context and perform operations on the database. The Handle method is where the logic for processing the Query request is implemented. In this case, it retrieves the list of activities from the database using Entity Framework Core's ToListAsync method and returns it as a response.
    public class Handler(AppDbContext context/*, ILogger<GetActivityList> logger*/, IMapper mapper) : IRequestHandler<Query, Result<List<ActivityDto>>>
    {
        // The purpose of the cancellation token is to allow the operation to be cancelled if it takes too long or if the client disconnects. By passing the cancellation token to the ToListAsync method, we can ensure that if the operation is cancelled, it will stop executing and free up resources. This is especially important for long-running operations that may consume a lot of resources or for operations that may be affected by network issues or client disconnections. By using a cancellation token, we can improve the responsiveness and reliability of our application. If the user cancels the request or if the request takes too long, the cancellation token will signal the operation to stop, allowing our application to handle the cancellation gracefully and avoid unnecessary resource consumption.
        // For example - if the user exits the page while the request is still processing, the cancellation token will signal the operation to stop, preventing it from continuing to run in the background and consuming resources unnecessarily.
        public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
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
            var result = await context.Activities.ProjectTo<ActivityDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            if (result == null)
            {
                return Result<List<ActivityDto>>.Failure("Failed to get activities", 400);
            }
            return Result<List<ActivityDto>>.Success(result);
        }
    }
}
