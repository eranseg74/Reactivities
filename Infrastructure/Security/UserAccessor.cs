using System;
using System.Security.Claims;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Infrastructure.Security;

// We need the IHttpContextAccessor because this will give us access to httpContext because it contains the user object that contains the user id and more.
public class UserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : IUserAccessor
{
    public async Task<User> GetUserAsync()
    {
        return await dbContext.Users.FindAsync(GetUserId()) ?? throw new UnauthorizedAccessException("No user is logged in");
    }

    public string GetUserId()
    {
        // Can get the claims in the User token. The user id inside the token is defined as thre NameIdentifier
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("No user found"); // Will be thrown if we do not have the HttpContext or the User
    }
}
