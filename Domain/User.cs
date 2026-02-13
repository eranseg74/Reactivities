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
    public ICollection<Photo> Photos { get; set; } = [];
    // Note! Defining the Photos alone will not create the relation between the tables. This means that if we will delete a user, the photos will not be deleted. This is because we have not defined the relation between the tables. We need to define the relation in the Photo entity as well. Only then will EF be able to create the relation between the tables and delete the photos when the user is deleted. It is not always desired to delete the photos when the user is deleted, but in this case we want to do that, so we will define the relation in the Photo entity as well (User and UserId).
}
