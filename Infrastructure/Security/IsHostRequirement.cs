using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

// The IsHostRequirement class represents an authorization requirement that checks if the user is the host of an activity. It implements the IAuthorizationRequirement interface, which is a marker interface used to identify authorization requirements.
public class IsHostRequirement : IAuthorizationRequirement // Represents an authorization requirement.
{

}

// The HttpContextAccessor is used to access the current HTTP context, which can provide information about the user and the request. The AppDbContext is used to access the database, which may be necessary to check if the user is the host of an activity.
public class IsHostRequirementHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<IsHostRequirement>
{
    // The AuthorizationHandlerContext provides information about the authorization process, including the user and the requirements being evaluated. The IsHostRequirement is the specific requirement that we are checking against. In this method, you would typically implement the logic to determine if the user is the host of the activity, which may involve querying the database using the AppDbContext and accessing user information from the HttpContext.
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return;
        }
        // Gets or sets the current IHttpContextAccessor.HttpContext. Returns null if there is no active IHttpContextAccessor.HttpContext.
        var httpContext = httpContextAccessor.HttpContext;
        // The GetRouteValue method is used to retrieve a value from the route data. In this case, we are trying to get the "id" value from the route, which represents the activity ID. If the "id" value is not present in the route, we return from the method, as we cannot proceed with the authorization check without it. If it does exist, we can then use it to check if the user is the host of the activity in the database. The is not string activityId part is a pattern matching expression that checks if the value retrieved from the route is a string and assigns it to the activityId variable if it is. If the value is not a string, the condition will evaluate to true, and we will return from the method. This ensures that we only proceed with the authorization check if we have a valid activity ID to work with.
        if (httpContext?.GetRouteValue("id") is not string activityId)
        {
            return;
        }
        // The change tracker will not track any of the entities that are returned from a LINQ query. If the entity instances are modified, this will not be detected by the change tracker and DbContext.SaveChanges() will not persist those changes to the database.
        // Disabling change tracking is useful for read - only scenarios because it avoids the overhead of setting up change tracking for each entity instance.You should not disable change tracking if you want to manipulate entity instances and persist those changes to the database using DbContext.SaveChanges().

        // We have to add it here because otherwise, EF will track this field and since we use the FindAsync method in the EditActivity which does not return navigation properties, the list will be empty (var activity = await context.Activities.FindAsync([request.ActivityDto.Id], cancellationToken: cancellationToken);). After we get the data we map it back to the activity (mapper.Map(request.ActivityDto, activity);) so we are basically reseting the attendees. Adding the AsNoTracking will force EF not to track this field so when we execute the SaveChanges, this field will not be updated
        var attendee = await dbContext.ActivityAttendees.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId && x.ActivityId == activityId);

        if (attendee == null)
        {
            return;
        }
        if (attendee.IsHost)
        {
            // Indicates that the authorization requirement has been successfully evaluated and met. This will allow the user to access the resource or perform the action that requires this authorization. The Succeed method is typically called when the authorization logic determines that the user meets the necessary criteria for access, such as being the host of an activity in this case. By calling context.Succeed(requirement), we are signaling that the user has successfully met the IsHostRequirement, and they should be granted access to the protected resource or action.
            context.Succeed(requirement);
        }
    }
}
