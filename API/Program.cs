using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Services - If we want a certain functionallity we can define it in a class. To use this functionallity inside the API we need to inject it as a dependency. This way it will be on the dotnet responsibility to create instances from this class when required, and also despose of it when not required any more

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi(); // Package removed
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    // opt refers to the DbContextOptionsBuilder instance that is used to configure the database context.
    // Here, we are using the UseSqlite method to specify that we want to use SQLite as our database provider.
    // We are also retrieving the connection string from the configuration file using builder.Configuration.GetConnectionString("DefaultConnection").
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection(); // No need since we are running only on HTTPS

// app.UseAuthorization();

app.MapControllers(); // This is the line that tells the API to use the controllers we defined. Without this line, the API will not be able to handle any requests and will return a 404 error for all requests.

// The using keyword ensures that the scope is disposed of correctly after use.
// This is important for managing the lifetime of services and ensuring that resources are released properly.
// Here, we are creating a scope to get the required services for seeding the database.
// The CreateScope method creates a new scope for the services, and we can then use the ServiceProvider to get the required services within that scope.
// A scope is a way to manage the lifetime of services in a controlled manner.
// When we start the application we run the following code, get hold of the DbContext, use the MigrateAsync to run a migration and create the DB if required, and once we have the DB we seed the data (incase the DB wse created)
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider; // Get the service provider from the scope and use it to get the required services.
try
{
    // Seed the database with initial data. The GetRequiredService method retrieves the AppDbContext instance from the service provider which gives us a connection to the database so we can seed it.
    var context = services.GetRequiredService<AppDbContext>();
    // The MigrateAsync asynchronously applies any pending migrations for the context to the database. Will create the database if it does not already exist.
    await context.Database.MigrateAsync(); // Apply any pending migrations to the database. This ensures that the database schema is up to date before seeding data. In this way we don't have to manually run the migrations using CLI commands (dotnet ef database update...).
    await DbInitializer.SeedData(context); // seed the database with initial data.
}
catch (Exception ex)
{
    // Here we are logging any exceptions that occur during the migration or seeding process. The ILogger<Program> service is used to log the error message. ILooger is a generic interface for logging where the category name is derived from the specified Program type name. Generally used to enable activation of a named ILogger from dependency injection.
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
