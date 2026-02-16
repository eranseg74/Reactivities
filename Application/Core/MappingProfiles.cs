using Application.Activities.DTOs;
using Application.Profiles.DTOs;
using AutoMapper;
using Domain;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // We want to map if the current user is following the received user ("Am I following the received user?"). Since we do not know here the current user's identity we define this optional string and set it to null.
        // In the mapping configuration between the User and the UserProfile we map between the boolean variable Following and the result of checking if this currentUserId is in the Followers collection of the source, which is a User. Any way, we need to pass the currentUserId and we do so by passing it as a second parameter in the '.ProjectTo()' as defined in the GetFollowings class, in the "followers" case (In the switch statement) as a new object (new {currentUserId = userAccessor.GetUserId()}).
        string? currentUserId = null;

        // The CreateMap method is used to create a mapping configuration between the source type (Activity) and the destination type (Activity). This means that AutoMapper will know how to map properties from an Activity object to another Activity object. In this case, since the source and destination types are the same, AutoMapper will simply copy the values of the properties from one Activity object to another when we use the Map method in our handlers. This is useful in scenarios like editing an activity, where we want to update an existing activity with new values without having to manually copy each property.
        CreateMap<Activity, Activity>();
        CreateMap<CreateActivityDto, Activity>();
        CreateMap<EditActivityDto, Activity>();
        // POSSIBLE OBJECT CYCLE
        // When we run the following API call:
        /*
        var activity = await context.Activities
            .Include(x => x.Attendees) // This will include the ActivityAttendee data related to the selected activity
            .ThenInclude(x => x.User) // This will include the users that are attendees of the selected activity
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        */
        // We get a 500 Internal Server Error indicating on a possible object cycle. This is due to the fact that we include the Attendees in the call. When sending the data to the client the system cannot send a C# object so it attempts to serialize it. During serialization of the activity object it encounters the ActivityAttendees collection included in this call. It then tries to serialize the collection of ActivityAttendee which contain a User object. The User object also contains a collection of ActivityAttendee so again it tries serializing the collection and so on so we get an infinite loop.
        // To handle it we created an ActivityDto and a UserProfile (also added the HostId and HostDisplayName to the ActivityDto so we can use it here) and changed the type of the Attendees from a collection of ActivityAttendee to a collection of UserProfile to avoid the loop.
        // Next we set the following mapping from the Activity to the ActivityDto but set that the mapping will be only on a single member - The HostDisplayName which will get its value from the collection of the ActivityAttendee where the IsHost is true and take the display name from that result.
        // We can st the ! sign to avoid the warning of possible null since automapper will return null if the host is null and not throw an exception so it is fine.
        CreateMap<Activity, ActivityDto>()
            .ForMember(d => d.HostDisplayName, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost)!.User.DisplayName))
            // Doing the same for the HostId
            .ForMember(d => d.HostId, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost)!.User.Id));
        // We also need to map from the ActivityAttendee to the UserProfile
        CreateMap<ActivityAttendee, UserProfile>()
            .ForMember(d => d.Bio, o => o.MapFrom(s => s.User.Bio))
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.User.DisplayName))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.User.ImageUrl))
            .ForMember(d => d.Id, o => o.MapFrom(s => s.User.Id))
            .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.User.Followers.Count))
            .ForMember(d => d.FollowingsCount, o => o.MapFrom(s => s.User.Followings.Count))
            .ForMember(d => d.Following, o => o.MapFrom(s => s.User.Followers.Any(x => x.Observer.Id == currentUserId)));

        // Important!!! Do not call CreateMap twice or more for the same mapping. This will return null! Just chain the .ForMember() call on the same mapping (see above). The following will not work:
        /*
        CreateMap<Activity, ActivityDto>().ForMember(d => d.HostDisplayName, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost)!.User.DisplayName));
        CreateMap<Activity, ActivityDto>().ForMember(d => d.HostId, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost)!.User.Id));
        */
        CreateMap<User, UserProfile>()
            .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
            .ForMember(d => d.FollowingsCount, o => o.MapFrom(s => s.Followings.Count))
            .ForMember(d => d.Following, o => o.MapFrom(s => s.Followers.Any(x => x.Observer.Id == currentUserId)));

        // The Id, Body, and CreateAt are identical in both the Comment and the CommentDto and are not navigation properties so there is no need to specify them.
        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.User.DisplayName))
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.User.Id))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.User.ImageUrl));

        CreateMap<Activity, UserActivityDto>();
    }
}
