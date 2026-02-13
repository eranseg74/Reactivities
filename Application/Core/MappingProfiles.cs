using Application.Activities.DTOs;
using Application.Profiles.DTOs;
using AutoMapper;
using Domain;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
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
            .ForMember(d => d.Id, o => o.MapFrom(s => s.User.Id));

        // Important!!! Do not call CreateMap twice or more for the same mapping. This will return null! Just chain the .ForMember() call on the same mapping (see above). The following will not work:
        /*
        CreateMap<Activity, ActivityDto>().ForMember(d => d.HostDisplayName, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost)!.User.DisplayName));
        CreateMap<Activity, ActivityDto>().ForMember(d => d.HostId, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost)!.User.Id));
        */
        CreateMap<User, UserProfile>();
    }
}
