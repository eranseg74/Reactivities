using API.Middleware;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using FluentValidation;
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

// Adds cross-origin resource sharing services to the specified IServiceCollection.
// Returns: The IServiceCollection so that additional calls can be chained.
// Requires also setting in the middleware section below
builder.Services.AddCors();

// Adding MediatR to the service collection to enable the mediator pattern in the application. The RegisterServicesFromAssemblyContaining method is used to register all the handlers, requests, and notifications defined in the specified assembly. In this case, we are registering the services from the assembly that contains the GetActivityList.Handler class.
builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODAxODcyMDAwIiwiaWF0IjoiMTc3MDM3Mzg1MyIsImFjY291bnRfaWQiOiIwMTljMzI3YzhlYTY3OTcyOTlmYzM0OTkwNjdjYTQwOCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2dzODJjZXczODBlc2Y3ZWR4dmd5M3YxIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.0ykoDAmRgB5Y-euiUc1Lg5cmABPdgEQR8qMov-H81kS39K0G03EaC5pw4zLukZi-tICs0FBYbCfbp4S_TS2-OF0C1SfZjUrSf79UaNJBli4rvmMCKPJ17cJRIITngdzRHPM-694j3ZgnFxcMyq8wvqbGw9nXac4jgK_z0-32_kPAUhO8CovFWWvB1sSlbkBXFmA2VzE-h4MgSBJsPAr3McwSvA1iZ9eTzWHaQbnMCUeOUZaoUCprgzB7uvNslrLI6_zGjrxzdrAsZDQOjDs_ndqGZdhTNmmKnXqDZx624gEBbDzuW869u-Jj9m1j9fTVPBfZevYpuMa993zUkYqKbg";
    cfg.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
    // Adding the validation behavior to the MediatR pipeline. This will ensure that all requests that go through the mediator will be validated using the FluentValidation validators we defined in the application. The AddOpenBehavior method is used to add a behavior to the MediatR pipeline, and we specify the type of the behavior as ValidationBehavior<,> which is a generic class that takes two type parameters: the request type and the response type. By adding this behavior to the pipeline, we can ensure that all requests are validated before they are handled by their respective handlers.
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4ODNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODAxODcyMDAwIiwiaWF0IjoiMTc3MDM3Mzg1MyIsImFjY291bnRfaWQiOiIwMTljMzI3YzhlYTY3OTcyOTlmYzM0OTkwNjdjYTQwOCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2dzODJjZXczODBlc2Y3ZWR4dmd5M3YxIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.0ykoDAmRgB5Y-euiUc1Lg5cmABPdgEQR8qMov-H81kS39K0G03EaC5pw4zLukZi-tICs0FBYbCfbp4S_TS2-OF0C1SfZjUrSf79UaNJBli4rvmMCKPJ17cJRIITngdzRHPM-694j3ZgnFxcMyq8wvqbGw9nXac4jgK_z0-32_kPAUhO8CovFWWvB1sSlbkBXFmA2VzE-h4MgSBJsPAr3McwSvA1iZ9eTzWHaQbnMCUeOUZaoUCprgzB7uvNslrLI6_zGjrxzdrAsZDQOjDs_ndqGZdhTNmmKnXqDZx624gEBbDzuW869u-Jj9m1j9fTVPBfZevYpuMa993zUkYqKbg";
    cfg.AddMaps(typeof(MappingProfiles).Assembly);
});

// Adding the Fluent validators services
// This adds all validators in the assembly of the type specified by the generic parameter
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();

builder.Services.AddTransient<ExceptionMiddleware>(); // Add the custom exception handling middleware to the service collection. This allows us to use this middleware in the HTTP request pipeline to handle exceptions that occur during the processing of requests and return appropriate error responses to the client.

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>(); // Use the custom exception handling middleware in the HTTP request pipeline. This will ensure that any exceptions that occur during the processing of requests are caught and handled by this middleware, allowing us to return appropriate error responses to the client and improve the overall robustness of the application. Note that this must be positioned at the top of the middleware pipeline to ensure that it can catch exceptions from all subsequent middleware and request processing. By placing it at the top, we can ensure that any exceptions that occur during the processing of requests are caught and handled by this middleware, allowing us to return appropriate error responses to the client and improve the overall robustness of the application.

// Configure the HTTP request pipeline.
// The order in the middleware section is important because these middleware will be executed by their order in each request.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection(); // No need since we are running only on HTTPS

// app.UseAuthorization();
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000", "https://localhost:3000"));
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
