namespace Domain;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public required string UserId { get; set; }
    // The null! operator is used to tell the compiler that we are sure that the User property will not be null when we access it, even though it is not initialized in the constructor. This is necessary because the User property is a navigation property that will be populated by Entity Framework when we query the database, and it cannot be initialized in the constructor since it depends on the database context which means that we cannot define it here as required. Otherwise, when we will create a new Comment object we will get an error: "Required member <Member> must be set in the object initializer or attribute constructor"
    public User User { get; set; } = null!;
    public required string ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;
}
