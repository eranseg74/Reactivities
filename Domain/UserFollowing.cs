namespace Domain;

public class UserFollowing
{
    // Navigation properties
    public required string ObserverId { get; set; }
    public User Observer { get; set; } = null!;
    public required string TargetId { get; set; }
    public User Target { get; set; } = null!;

}
