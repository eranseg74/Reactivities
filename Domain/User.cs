using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties (for defining relation with the Activity entity). This, along with the navigation property of Attendees in the Activity entity will create the many to many relation between the tables.
    // Note that in this case we are defining the joined table manually! This is why the type is the manully defined table. Otherwise the type would be Activity and EF would create the joined table automatically.
    public ICollection<ActivityAttendee> Activities { get; set; } = [];
}
