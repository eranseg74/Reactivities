using Application.Activities.DTOs;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // The CreateMap method is used to create a mapping configuration between the source type (Activity) and the destination type (Activity). This means that AutoMapper will know how to map properties from an Activity object to another Activity object. In this case, since the source and destination types are the same, AutoMapper will simply copy the values of the properties from one Activity object to another when we use the Map method in our handlers. This is useful in scenarios like editing an activity, where we want to update an existing activity with new values without having to manually copy each property.
            CreateMap<Activity, Activity>();
            CreateMap<CreateActivityDto, Activity>();
            CreateMap<EditActivityDto, Activity>();
        }
    }
}
