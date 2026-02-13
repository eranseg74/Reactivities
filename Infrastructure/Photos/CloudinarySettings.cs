namespace Infrastructure.Photos;

public class CloudinarySettings
{
    // Make sure the names are exactly as defined in the appsettings.json file. Otherwise, it will not work and there will be no notification about it
    public required string CloudName { get; set; }
    public required string ApiKey { get; set; }
    public required string ApiSecret { get; set; }
}
