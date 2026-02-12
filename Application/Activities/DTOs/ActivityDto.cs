using Application.Profiles.DTOs;

namespace Application.Activities.DTOs;

public class ActivityDto
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public DateTime Date { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public bool IsCancelled { get; set; }
    public required string HostDisplayName { get; set; }
    public required string HostId { get; set; }

    // location details
    public required string City { get; set; }
    public required string Venue { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Navigation properties (for defining relation with the User entity). This, along with the navigation property of Activities in the User entity will create the many to many relation between the tables.
    // Note that in this case we are defining the joined table manually! This is why the type is the manully defined table. Otherwise the type would be User and EF would create the joined table automatically
    public ICollection<UserProfile> Attendees { get; set; } = [];
}
