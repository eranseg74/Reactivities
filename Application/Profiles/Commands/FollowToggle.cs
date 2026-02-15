using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class FollowToggle
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string TargetUserId { get; set; }
    }

    public class Handler(AppDbContext dbContext, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var observer = await userAccessor.GetUserAsync();
            var target = await dbContext.Users.FindAsync([request.TargetUserId], cancellationToken);
            if (target == null)
            {
                return Result<Unit>.Failure("Target user not found", 400);
            }
            // [observer.Id, target.Id] is the primary key used in the UserFoolowings table. The following line checks if the current user is following the target according to the TargetUserId. If so, it will be removed, if not - it will be added
            var following = await dbContext.UserFollowings.FindAsync([observer.Id, target.Id], cancellationToken);
            if (following == null)
            {
                dbContext.UserFollowings.Add(new UserFollowing
                {
                    ObserverId = observer.Id,
                    TargetId = target.Id
                });
            }
            else
            {
                dbContext.UserFollowings.Remove(following);
            }
            return await dbContext.SaveChangesAsync() > 0 ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating following", 400);
        }
    }
}
