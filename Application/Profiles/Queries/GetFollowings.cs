using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetFollowings
{
    public class Query : IRequest<Result<List<UserProfile>>>
    {
        // Predicate will be used to get either the followers list or the followings list
        public string Predicate { get; set; } = "followers";
        // Enabling to get any user's followings or followers
        public required string UserId { get; set; }
    }

    public class Handler(AppDbContext dbContext, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<List<UserProfile>>>
    {
        public async Task<Result<List<UserProfile>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var profiles = new List<UserProfile>();
            switch (request.Predicate)
            {
                case "followers":
                    profiles = await dbContext.UserFollowings
                        // Find all users in the UserFollowings table that their TargetId is the userId in the request
                        .Where(x => x.TargetId == request.UserId)
                        // From the given list select the Observer object (the following user)
                        .Select(x => x.Observer)
                        // Convert the followers from UserFollowing to UserProfile
                        .ProjectTo<UserProfile>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() })
                        // Fetch from the DB and return a list
                        .ToListAsync(cancellationToken);
                    break;
                case "followings":
                    profiles = await dbContext.UserFollowings
                        // Find all users in the UserFollowings table that their TargetId is the userId in the request
                        .Where(x => x.ObserverId == request.UserId)
                        // From the given list select the Observer object (the following user)
                        .Select(x => x.Target)
                        // Convert the followers from UserFollowing to UserProfile
                        .ProjectTo<UserProfile>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() })
                        // Fetch from the DB and return a list
                        .ToListAsync(cancellationToken);
                    break;
            }
            return Result<List<UserProfile>>.Success(profiles);
        }
    }
}
