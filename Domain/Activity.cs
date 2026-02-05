namespace Domain
{
    public class Activity
    {
        // Unique identifier for the activity. Must be public because the Entity Framework needs to access it. Otherwise it will throw an error.
        // Because the name of the property is "Id", Entity Framework will automatically recognize it as the primary key for the Activities table in the database.
        public string Id { get; set; } = Guid.NewGuid().ToString(); // We are setting a default value for the Id property using Guid.NewGuid().ToString() to ensure that each activity has a unique identifier when created, and not rely on the database to generate it.
        public required string Title { get; set; }
        public DateTime Date { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }

        // location details
        public required string City { get; set; }
        public required string Venue { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
