# Reactivities

### Initial setup
This project uses Dotnet for Backend, and React for Frontend.
The setup contained the following steps:
- Create a new solution using the ```dotnet new sln``` command.
- Create a new webapi project using the ```dotnet new webapi -n API -controllers``` command. The `-n` is for setting a name to the project and the `controllers` means that the project uses the controller-based approach where controllers are classes that derive from the ControllerBase class.
- The project structure is as follows:
  - **Domain** - This layer will contain the basic entities. This layer does not depend on any other layers.
  - **Application** - This layer will hold all the business logic. This layer will communicate with the **Domain** and on the **Persistence** layers and therefore, depend on them.
  - **API** - This layer is responsible for getting requests from the client and forward them to the **Application** layer, and also send the responses back to the client. It communicates and depend on the **Application** layer.
  - **Persistence** - This layer is responsible to persist data. It depeneds on the **Domain** layer.

  All layers are defined in separate libraries using the ```dotnet new classlib -n <layer_name>``` command. The `-n` indicates the name of the new classlib.
- Adding the layers to the solution using the ```dotnet sln add <layer_name>``` command (**API**, **Application**, **Persistence**, and **Domain**).

Each entity in the domain will most likely represent a table in the database.

With databases we work with the DbContext class which allows us to communicate with the database. All the connections to the database will be handled by EntityFramework.

SqLite
To start using **SqLite** we need to first add the `Microsoft.EntityFrameworkCore.Sqlite` package from the **Nuget Gallery** to the [Persistence] project which is the layer that is responsible on the communication with the database (each layer is a project on its own)

Because we will create __first code__ migrations we need the package in the startup project which is the [API] project.

### Appsettings.Development.json and Appsettings.json
**Appsettings.json** will be read both in the production and development environments
**Appsettings.Development.json** will be read only in the development environments

## Migrations
To create a migration we need to do it in the root folder above the API and all other projects (in the Reactivities level in our case). Also, since we have multiple projects we need to specify two switches:
1) The project that contains the DbContext using the `-p` switch.
2) The startup project, using the `-s` switch.
The command is: ```dotnet ef migrations add <migration name> -p <project that contains the DbContext> -s <startup project>```
In our case - ```dotnet ef migrations add InitialCreate -p Persistense -s API```

To apply a migration we use the ```dotnet ef database update -p <project that contains the DbContext> -s <startup project>```. If the database was not created it will be created after executing this command.
The actual command is: ```dotnet ef database update -p Persistence -s API```. Here we also need the switches.

## Adding a gitignore file suitable for dotnet
We can use dotnet to create a gitignore file. This will create the file with all the file types that comes with dotnet and should be ignored when uploading to git. The command is - ```dotnet new gitignore```

## Setting up CORS (Cross Origin Resources)
CORS controlls how resources from different origins can be accessed. It is like a set of permissions that the API server specifies in order to tell the browsers which sites are allowed to make requests to them. So it must be configured on the server side (in the Program.cs file)

# CQRS - Command Query Responsibility Segragation (sometimes known as Command Query Separation)
Separates commands from queries
**Commands** - Does something, modifies State, and should not return a value.
**Queries** - Answers a question, does not modify a State, and should return a value.

# Mediator
Since the data flow is in one direction - From the API layer to the Application layer, to the Domain and Persistence layer, and from the Persistence to the Domain layer, if the API makes a call to the Application, the Application does not "know" about the API. To send the data back to the API layer we implement a Mediator which will be responsible to send a response (mediate) back to the layer that issued the request.
We use the MediatR package from thre Nuget Package gallery to mediate between the layers.

# AutoMapper
To work with AutoMapper first we need to install the `AutoMapper` Nuget package.
After installation we create a Core folder which will contain classes that are used in all of the project. This folder will contain a MappingProfile class in which we define the mapping strategies.
Also, we need to define it as a service in the `Program.cs` file


# -----------------------------------------------------------------------------------------------------------------
# FRONTEND - REACT
# -----------------------------------------------------------------------------------------------------------------

To create React project we use Vite using the ```npm create vite@latest``` command.

We can change the port number in the `vite.config.ts` file by setting the server object (before or next to the `plugins` object) that will contain `port: <port number>` pair

## Files Structure
Inside the client (react) project we define the app and features folders.
The **app** folder will use for more application specific that are not related to a specific feature, such as layouts, shared components, etc.
The **features** folder will contain all the features by folders (a folder for each feature).
The **public** folder adjacent to the **src** folder will contain all the assets (images, etc.) since when we will build the project the application will know to search for assets in this folder.
The **lib** folder will use to hold all non-react classes such types.