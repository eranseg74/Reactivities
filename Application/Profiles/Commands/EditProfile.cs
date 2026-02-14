using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Commands;

public class EditProfile
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string DisplayName { get; set; }
        public string? Bio { get; set; }
    }
    public class Handler(AppDbContext dbContext, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserAsync();
            if (string.IsNullOrEmpty(request.DisplayName))
            {
                return Result<Unit>.Failure("Missing DisplayName parameter", 400);
            }
            user.DisplayName = request.DisplayName;
            if (!string.IsNullOrEmpty(request.DisplayName))
            {
                user.Bio = request.Bio;
            }
            // Setting the user props as changed even if the properties are the same. This is to avoid EF from throwing an error in case the fields are not changed (the result in this case will be 0).
            dbContext.Entry(user).State = EntityState.Modified;
            var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem editing profile", 400);
        }
    }
}

