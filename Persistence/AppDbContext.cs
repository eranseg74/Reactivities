using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistence;

// A DbContext instance represents a session with the database and can be used to query and save instances of your entities. DbContext is a combination of the Unit Of Work and Repository patterns.
// It is a fundamental part of Entity Framework Core and is responsible for managing the database connections, tracking changes to entities, and coordinating database operations.
// We are inheriting from the DbContext class provided by Entity Framework Core to create our own database context class called AppDbContext. We also provide a constructor that takes DbContextOptions as a parameter and passes it to the base class constructor. The options are defined in the API's Program.cs file where we register the DbContext with the appropriate options as a service.
// public class AppDbContext(DbContextOptions options) : DbContext(options)
// Changing the derive class from DbContext to IdentityDbContext when working with Identity framework. Also assigning it a type parameter of User
public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    // The EntityFramework uses the DbSet<T> properties to know which entities we want to include in the model. Each DbSet<T> property corresponds to a table in the database, and the type parameter T represents the entity type that will be stored in that table. The name of the DbSet<T> property is used as the name of the table in the database (Activities in this case) unless we specify a different name using data annotations or Fluent API configurations. It will look at the Activity class to determine the columns and their types in the Activities table.
    public required DbSet<Activity> Activities { get; set; }
    public required DbSet<ActivityAttendee> ActivityAttendees { get; set; }
    public required DbSet<Photo> Photos { get; set; }
    public required DbSet<Comment> Comments { get; set; }
    public required DbSet<UserFollowing> UserFollowings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ActivityAttendee>(x => x.HasKey(a => new { a.ActivityId, a.UserId }));

        builder.Entity<ActivityAttendee>().HasOne(x => x.User).WithMany(x => x.Activities).HasForeignKey(x => x.UserId);
        builder.Entity<ActivityAttendee>().HasOne(x => x.Activity).WithMany(x => x.Attendees).HasForeignKey(x => x.ActivityId);

        // Configuring the relation for the UserFollowings
        builder.Entity<UserFollowing>(x =>
        {
            // Sets the property that make up the primary key for this entity type. In this case it is a combination of the ObserverId and the TargetId.
            x.HasKey(k => new { k.ObserverId, k.TargetId });

            // Configuring one side of the relation - A user can follow many users
            x.HasOne(o => o.Observer)
                .WithMany(f => f.Followings)
                .HasForeignKey(o => o.ObserverId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuring the other side of the relation - A user can be followed many users
            x.HasOne(o => o.Target)
                .WithMany(f => f.Followers)
                .HasForeignKey(o => o.TargetId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(), // Convert to UTC when saving to the database
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Specify that the DateTime is in UTC when reading from the database
        );

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }



}
