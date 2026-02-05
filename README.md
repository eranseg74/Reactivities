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
