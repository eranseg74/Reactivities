using Application.Core;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetUserActivities
{
    public class Query : IRequest<Result<List<UserActivityDto>>>
    {
        public required string UserId { get; set; }
        public required string FilterBy { get; set; }
    }

    public class Handler(AppDbContext dbContext, IMapper mapper) : IRequestHandler<Query, Result<List<UserActivityDto>>>
    {
        public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = dbContext.ActivityAttendees
                        .Where(x => x.UserId == request.UserId)
                        .OrderBy(x => x.Activity.Date)
                        .Select(x => x.Activity)
                        .AsQueryable();

            query = request.FilterBy switch
            {
                "past" => query.Where(x => x.Date <= DateTime.UtcNow && x.Attendees.Any(x => x.UserId == request.UserId)),
                "hosting" => query.Where(x => x.Attendees.Any(x => x.IsHost && x.UserId == request.UserId)),
                _ => query.Where(x => x.Date >= DateTime.UtcNow && x.Attendees.Any(x => x.UserId == request.UserId))
            };
            var projectedActivities = query.ProjectTo<UserActivityDto>(mapper.ConfigurationProvider);
            var activities = await projectedActivities.ToListAsync(cancellationToken);
            return Result<List<UserActivityDto>>.Success(activities);
        }
    }
}
