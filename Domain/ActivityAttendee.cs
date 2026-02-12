namespace Domain;

public class ActivityAttendee
{
    public string? UserId { get; set; }
    public User User { get; set; } = null!;
    public string? ActivityId { get; set; }
    public Activity Activity { get; set; } = null!; // We need to set the default value of the Activity property to null! because it is a non-nullable reference type, and we are not initializing it in the constructor. By setting it to null!, we are telling the compiler that we will ensure that it is not null at runtime, even though it is not initialized here. This is necessary because the Entity Framework will populate this property when it retrieves data from the database, and we want to avoid any warnings about uninitialized non-nullable reference types.

    // The above will be created automatically if the EF would create the joined table.
    // We also want the following props so we create and configure the joined table manually:
    public bool IsHost { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
}
