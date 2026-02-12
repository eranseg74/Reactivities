using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class UpdateAttendance
{
    // Unit represents a void type, since void is not a valid return type in C#. We do not need to return anything because the client has already all the data it needs when hitting this endpoint
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext dbContext, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Fetching the activity using the Eager loading approach
            var activity = await dbContext.Activities.Include(x => x.Attendees).ThenInclude(x => x.User).SingleOrDefaultAsync(x => request.Id == x.Id, cancellationToken: cancellationToken);
            // Both SingleOrDefaultAsync and FirstOrDefaultAsync are fine here. The difference between them is that if there several matches the firstOrDefault will return the first one, and the singleOrDefault will throw an error in that case
            if (activity == null)
            {
                return Result<Unit>.Failure("Activity not found", 404);
            }
            var user = await userAccessor.GetUserAsync(); // Fetching the current user data
            // Checking if the current user is attending in the fetched activity:
            var attendance = activity.Attendees.FirstOrDefault(x => x.UserId == user.Id);
            // Checking if the current user is the host of the activity.
            var isHost = activity.Attendees.Any(x => x.IsHost && x.UserId == user.Id);
            if (attendance != null) // Meaning that the user is participating in the activity
            {
                // Checking if the user is the host of the activity. In this case we toggle the IsCancelled so the host will be able to cancel the activity
                if (isHost)
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }
                // Ifthe user is not the host he will be able to remove himself from the activity
                else
                {
                    activity.Attendees.Remove(attendance);
                }
            }
            else // If the user is not attending the activity he will be able to add himselt
            {
                activity.Attendees.Add(new ActivityAttendee
                {
                    UserId = user.Id,
                    ActivityId = activity.Id,
                    IsHost = false
                });
            }
            var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating the DB", 400);
        }
    }
}
