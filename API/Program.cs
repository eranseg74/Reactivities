using API.Middleware;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Services - If we want a certain functionallity we can define it in a class. To use this functionallity inside the API we need to inject it as a dependency. This way it will be on the dotnet responsibility to create instances from this class when required, and also despose of it when not required any more

builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});
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

// Defining it as a scoped service because it has to be scoped to the http request itself
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4ODNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODAxODcyMDAwIiwiaWF0IjoiMTc3MDM3Mzg1MyIsImFjY291bnRfaWQiOiIwMTljMzI3YzhlYTY3OTcyOTlmYzM0OTkwNjdjYTQwOCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2dzODJjZXczODBlc2Y3ZWR4dmd5M3YxIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.0ykoDAmRgB5Y-euiUc1Lg5cmABPdgEQR8qMov-H81kS39K0G03EaC5pw4zLukZi-tICs0FBYbCfbp4S_TS2-OF0C1SfZjUrSf79UaNJBli4rvmMCKPJ17cJRIITngdzRHPM-694j3ZgnFxcMyq8wvqbGw9nXac4jgK_z0-32_kPAUhO8CovFWWvB1sSlbkBXFmA2VzE-h4MgSBJsPAr3McwSvA1iZ9eTzWHaQbnMCUeOUZaoUCprgzB7uvNslrLI6_zGjrxzdrAsZDQOjDs_ndqGZdhTNmmKnXqDZx624gEBbDzuW869u-Jj9m1j9fTVPBfZevYpuMa993zUkYqKbg";
    cfg.AddMaps(typeof(MappingProfiles).Assembly);
});

// Adding the Fluent validators services
// This adds all validators in the assembly of the type specified by the generic parameter
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();

builder.Services.AddTransient<ExceptionMiddleware>(); // Add the custom exception handling middleware to the service collection. This allows us to use this middleware in the HTTP request pipeline to handle exceptions that occur during the processing of requests and return appropriate error responses to the client.

// Adding the Identity framework. This will add the necessary services for authentication and authorization using Identity. We specify the type of the user as User, which is our custom user class that inherits from IdentityUser. We also configure some options for the Identity framework, such as requiring unique email addresses for users. Finally, we specify that we want to use Entity Framework Core to store the identity data, and we specify our AppDbContext as the context to use for this purpose. This will allow us to manage users, roles, and other identity-related data in our database using Entity Framework Core.
builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

// Adding the authorization service for authorizing activities only as host
// We use this policy when we want to update an activity because we want to enforce that only the host of an activity can update it
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IsActivityHost", policy =>
    {
        policy.Requirements.Add(new IsHostRequirement());
    });
});
// The AddTransient method is used to register the IsHostRequirementHandler as a transient service for the IAuthorizationHandler interface. This means that a new instance of the IsHostRequirementHandler will be created each time it is requested. The IAuthorizationHandler interface is used by the authorization system to evaluate authorization requirements, and by registering our custom handler, we can implement the logic to check if a user is the host of an activity when the "IsActivityHost" policy is evaluated during authorization checks in our controllers.
builder.Services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

// Configure the CloudinarySettings class to be used for dependency injection. This will allow us to inject the CloudinarySettings class into our services and controllers to access the Cloudinary configuration settings defined in the appsettings.json file. The Configure method is used to bind the CloudinarySettings class to the corresponding section in the configuration file, which allows us to easily access these settings throughout our application using dependency injection.
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

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
// CORS (Cross-Origin Resource Sharing) is a security feature implemented by web browsers to restrict web applications running on one origin (domain) from accessing resources on a different origin. This is done to prevent malicious websites from making unauthorized requests to other websites on behalf of the user. By configuring CORS in our API, we can specify which origins are allowed to access our API and what types of requests they can make. In this case, we are allowing any header, any method, and credentials (cookies) from the specified origins (http://localhost:3000 and https://localhost:3000). This means that our API will accept requests from these origins and allow them to include credentials such as cookies in their requests. This is important for enabling authentication and maintaining user sessions when making requests from the client application running on these origins.
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000", "https://localhost:3000"));

// The order is important! We have to authenticate the user first before we can authorize the user to do what he needs to do
app.UseAuthentication(); // Use the authentication middleware to enable authentication in the application. This will allow us to authenticate users and protect certain endpoints that require authentication. By adding this middleware, we can ensure that only authenticated users can access protected resources and perform certain actions in the application, enhancing security and controlling access to sensitive data and functionality.
app.UseAuthorization(); // Use the authorization middleware to enable authorization in the application. This will allow us to authorize users based on their roles and permissions, and protect certain endpoints that require specific roles or permissions. By adding this middleware, we can ensure that only authorized users can access protected resources and perform certain actions in the application, enhancing security and controlling access to sensitive data and functionality.

app.MapControllers(); // This is the line that tells the API to use the controllers we defined. Without this line, the API will not be able to handle any requests and will return a 404 error for all requests.

// Mapping the API Identity endpoint. This will automatically create endpoints for user registration, login, logout, and other identity-related operations based on the Identity framework. By mapping these endpoints, we can easily manage user authentication and authorization in our application without having to manually create these endpoints ourselves. The MapIdentityApi method is a convenient way to set up the necessary endpoints for user management and authentication using the Identity framework. We specify the type of the user as User, which is our custom user class that inherits from IdentityUser. This will allow us to manage users, roles, and other identity-related data in our database using Entity Framework Core and the Identity framework, and it will provide us with a set of endpoints to handle user registration, login, logout, and other identity-related operations out of the box. By using this method, we can save time and effort in setting up the necessary endpoints for user management and authentication in our application, and we can focus on implementing the core functionality of our application while still having robust user authentication and authorization features provided by the Identity framework. The "api" means that we will have to add "api" to the URL - //.../api/login (like all of our other endpoints)
app.MapGroup("api").MapIdentityApi<User>();

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
    var userManager = services.GetRequiredService<UserManager<User>>();
    // The MigrateAsync asynchronously applies any pending migrations for the context to the database. Will create the database if it does not already exist.
    await context.Database.MigrateAsync(); // Apply any pending migrations to the database. This ensures that the database schema is up to date before seeding data. In this way we don't have to manually run the migrations using CLI commands (dotnet ef database update...).
    await DbInitializer.SeedData(context, userManager); // seed the database with initial data.
}
catch (Exception ex)
{
    // Here we are logging any exceptions that occur during the migration or seeding process. The ILogger<Program> service is used to log the error message. ILooger is a generic interface for logging where the category name is derived from the specified Program type name. Generally used to enable activation of a named ILogger from dependency injection.
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
