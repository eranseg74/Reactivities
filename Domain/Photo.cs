using System.Text.Json.Serialization;

namespace Domain;

public class Photo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Url { get; set; }
    public required string PublicId { get; set; }

    // Navigation properties
    // These navigation properties, along with the Photos prop in the User entity will create a connection between the tables so when a user will be deleted, all of his photos will be deleted as well. It is not always recommended and we could implement a different approach of leaving the photos in Cloudinary and schedule a removal process of orphan photos in the storage. If we do not want this dependency just remove the following 2 navigation properties:
    public required string UserId { get; set; }

    // Note that in this case, like in the User and ActivityAttendee relation, where we had a 500 Internal Server Error caused by an infinite loop, we have the same situation. In this case, since wedo not need the user object returned we add the JsonIgnore annotation which will ignore this property when serializing the photo from a C# object to JSON.
    [JsonIgnore]
    public User User { get; set; } = null!;
}
