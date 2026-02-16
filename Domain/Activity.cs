using Microsoft.EntityFrameworkCore;

namespace Domain;

// We are adding the Index annotation because we are using pagination so if we want to increase the efficiency of fetching the desired batch of items each time we can index the Date field. By defaul EF indexes the Id property so we need to index any other property manually. Note that this annotation has to be on the Class level.
// Important!!! Adding Index requires migration!!!
[Index(nameof(Date))]
public class Activity
{
    // Unique identifier for the activity. Must be public because the Entity Framework needs to access it. Otherwise it will throw an error.
    // Because the name of the property is "Id", Entity Framework will automatically recognize it as the primary key for the Activities table in the database.
    public string Id { get; set; } = Guid.NewGuid().ToString(); // We are setting a default value for the Id property using Guid.NewGuid().ToString() to ensure that each activity has a unique identifier when created, and not rely on the database to generate it.
    public required string Title { get; set; }
    public DateTime Date { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public bool IsCancelled { get; set; }

    // location details
    public required string City { get; set; }
    public required string Venue { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Navigation properties (for defining relation with the User entity). This, along with the navigation property of Activities in the User entity will create the many to many relation between the tables.
    // Note that in this case we are defining the joined table manually! This is why the type is the manully defined table. Otherwise the type would be User and EF would create the joined table automatically
    public ICollection<ActivityAttendee> Attendees { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}
