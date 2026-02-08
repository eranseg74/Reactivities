# Reactivities

# -----------------------------------------------------------------------------------------------------------------
## *BACKEND - DOTNET*
# -----------------------------------------------------------------------------------------------------------------

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

### Migrations
To create a migration we need to do it in the root folder above the API and all other projects (in the Reactivities level in our case). Also, since we have multiple projects we need to specify two switches:
1) The project that contains the DbContext using the `-p` switch.
2) The startup project, using the `-s` switch.
The command is: ```dotnet ef migrations add <migration name> -p <project that contains the DbContext> -s <startup project>```
In our case - ```dotnet ef migrations add InitialCreate -p Persistense -s API```

To apply a migration we use the ```dotnet ef database update -p <project that contains the DbContext> -s <startup project>```. If the database was not created it will be created after executing this command.
The actual command is: ```dotnet ef database update -p Persistence -s API```. Here we also need the switches.

### Adding a gitignore file suitable for dotnet
We can use dotnet to create a gitignore file. This will create the file with all the file types that comes with dotnet and should be ignored when uploading to git. The command is - ```dotnet new gitignore```

### Setting up CORS (Cross Origin Resources)
CORS controlls how resources from different origins can be accessed. It is like a set of permissions that the API server specifies in order to tell the browsers which sites are allowed to make requests to them. So it must be configured on the server side (in the Program.cs file)

### CQRS - Command Query Responsibility Segragation (sometimes known as Command Query Separation)
Separates commands from queries
**Commands** - Does something, modifies State, and should not return a value.
**Queries** - Answers a question, does not modify a State, and should return a value.

### Mediator
Since the data flow is in one direction - From the API layer to the Application layer, to the Domain and Persistence layer, and from the Persistence to the Domain layer, if the API makes a call to the Application, the Application does not "know" about the API. To send the data back to the API layer we implement a Mediator which will be responsible to send a response (mediate) back to the layer that issued the request.
We use the MediatR package from thre Nuget Package gallery to mediate between the layers.

### AutoMapper
To work with AutoMapper first we need to install the `AutoMapper` Nuget package.
After installation we create a Core folder which will contain classes that are used in all of the project. This folder will contain a MappingProfile class in which we define the mapping strategies.
Also, we need to define it as a service in the `Program.cs` file.

### Error Handling - Server side
**Dato annotation**
DTO - Data Transfer Object - allow us to receive the required data from the client
A DTO is basically an object that lets us encapsulate data in order to send it from one layer to another (for example - sending data from Postman to our Application layer).
In the DTO classes we can define Data Annotations that will help us validate the data coming from the client.
This will give us much better validation! From an error that will look like this (no data annotations):
``` C#
// More Code...
"errors": {
    "$": [
        "JSON deserialization for type 'Application.Activities.DTOs.CreateActivityDto' was missing required properties including: 'title', 'description', 'category', 'city', 'venue'."
    ],
    "activityDto": [
        "The activityDto field is required."
    ]
},
// More Code...
```

To something like this (with data annotations):
```C#
// More Code...
"errors": {
    "City": [
        "The City field is required."
    ],
    "Title": [
        "The Title field is required."
    ],
    "Venue": [
        "The Venue field is required."
    ],
    "Category": [
        "The Category field is required."
    ],
    "Description": [
        "The Description field is required."
    ]
},
// More Code...
```

### Fluent Validation
The Fluent Validation is a Nuget package that is a better tool for data validation than just relying on the ApiController which is responsible for the validation via the data annotations.
In the Nuget Gallery select the `FluentValidation.DependencyInjectionExtensions` which is the Fluent validation package with dependency injection. Here it is installed in the Application project where all of the DTOs, Commands, and Queries are.

To start working with it, we create a Validators folder that will contain all the validator files for each desired object. In the validator file we specify rules that must be applied for the object to be validated.

Fluent works well with Mediator so we can catch errors comming from the Fluent Validators in the Mediator middleware.

One approach is to inject the IValidator and validate for each command and this might be too much. A better approach is to catch and handle all the validation errors in a middleware since we already defined it as a service in the *`Program.cs`* file.

In this project we define a ValidatorBehavior class in the **Application/Core** folder.

The steps for implementing the Fluent Validation component are as follows:

1. Install the Fluent Nuget package as described above.
2. If integrated with MediatR, create a generic ValidationBehavior class that will be used as a middleware.
3. Add the class as a service in the *`Program.cs`* file - `cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));`. The type will be <,> to specify a generic type.
4. Create the desired validator (for example - the `CreateActivityValidator` validator class with the desired rules).
5. Create our own middleware to handle the errors thrown by the validator ( in this project it is the `ExceptionMiddleware` class). This is because it is best to return the errors as http response and not plain exceptions. The error handling file will be in the **API** since this layer is responsible to forward requests from the client to the applicstion layer, and responses back to the client.
6. Add the custom middleware as a service, defining it as `Transient` (meaning that this object will be instanciated when an exception pops, and destroyed once the exception ended).
7. Add the middleware in the middlewares section in the Program.cs file.
8. Optinal - Create a `Result` class to provide a standardized way to represent the outcome of an operation, including whether it was successful or not, any relevant data or error messages, and an optional status code (see the Result class under the **Application/Core** folder).
9. Use the Result in the handlers (for example - in our `GetActivityDetails` handles in the **Application/Activities/Queries** folder).
10. Create a `HandleResult` method in the `BaseApiController` controller in order to provide a standardized way to handle the results returned by our handlers and convert them into appropriate HTTP responses.
11. Wrap the returned responses in the controllers with the `HandleResult` method.



# -----------------------------------------------------------------------------------------------------------------
## FRONTEND - REACT
# -----------------------------------------------------------------------------------------------------------------

To create React project we use Vite using the ```npm create vite@latest``` command.

We can change the port number in the `vite.config.ts` file by setting the server object (before or next to the `plugins` object) that will contain `port: <port number>` pair

### Files Structure
Inside the client (react) project we define the app and features folders.
The **app** folder will use for more application specific that are not related to a specific feature, such as layouts, shared components, etc.
The **features** folder will contain all the features by folders (a folder for each feature).
The **public** folder adjacent to the **src** folder will contain all the assets (images, etc.) since when we will build the project the application will know to search for assets in this folder.
The **lib** folder will use to hold all non-react classes such types.
Inside the __lib__ folder we add a __hooks__ folder. It will contain all the custom hooks we create. The purpose of the custom hooks is to take away all the logic in the components so they will include only the content that needs to be displayed.

### React Query
TanStack Query (formerly known as React Query) is often described as the missing data-fetching library for web applications, but in more technical terms, it makes fetching, caching, synchronizing and updating server state in your web applications a breeze.
While most traditional state management libraries are great for working with client state, they are not so great at working with async or server state. This is because server state is totally different. For starters, server state:
- Is persisted remotely in a location you may not control or own
- Requires asynchronous APIs for fetching and updating
- Implies shared ownership and can be changed by other people without your knowledge
- Can potentially become "out of date" in your applications if you're not careful

Once you grasp the nature of server state in your application, even more challenges will arise as you go, for example:
- Caching... (possibly the hardest thing to do in programming)
- Deduping multiple requests for the same data into a single request
- Updating "out of date" data in the background
- Knowing when data is "out of date"
- Reflecting updates to data as quickly as possible
- Performance optimizations like pagination and lazy loading data
- Managing memory and garbage collection of server state
- Memoizing query results with structural sharing

To use it we need to install the following package - ```npm i @tanstack/react-query```

It is recommended to also use our ESLint Plugin Query to help you catch bugs and inconsistencies while you code. You can install it via: ```npm i -D @tanstack/eslint-plugin-query```

React Query comes with a DevTool that help visualize all the inner workings of React Query and will likely save you hours of debugging if you find yourself in a pinch! Installation via - ```npm i @tanstack/react-query-devtools```

The first steps of using React Query is always to create a `queryClient` and wrap the application in a <QueryClientProvider>. When doing server rendering, it's important to create the queryClient instance inside of your app, in __React__ state (an instance ref works fine too). This ensures that data is not shared between different users and requests, while still only creating the queryClient once per component lifecycle.

Note that the <ReactQueryDevTtools /> also needs the query client so it has to be inside the <QueryClientProvider>

### React Router
To set up React Router we need to install the following package: ```npm i react-router```

Aftetr installation we create a Routes.tsx file (in this project it is inside the app/router folder)
In this file we will define all of the routes in the application. Each route contains a path and the element that will be displayed if the specified path mathes the current path.
We create a router object using the `createBrowserRouter` function which gets an array of objects as a prameter, where each object specifies a route.
A route can have children - will show in the path. For example - `/` might point to the <App /> component and `/activities` which will refer to the <ActivityDashboard /> might be a child of the <App /> component

React router gives us abilities such as getting the params (useParams), getting the query parameters (useSearchParams), navigation (useNavigation), our current location (useLoacation), and more.

After creating the router in the Routes.tsx file we swap the <App /> component with the router provider <RouterProvider router={router} />. The router is the object created in the Routes.tsx file

### MobX
__MobX__ is a signal based, battle-tested library that makes state management simple and scalable by transparently applying functional reactive programming.

#### Installation:
Requires installation of 2 packages, mobx and mobx-react-lite - ```npm install mobx mobx-react-lite```

#### Steps to Implement MobX:
1. **Install MobX and React Bindings**:
> Install MobX and the necessary React bindings (usually mobx-react-lite for functional components).

>```npm install mobx mobx-react-lite```

2. **Define the Data Store (Observable State)**:
> Creating a class or object to hold the application state. Use makeAutoObservable(this) in the constructor to automatically make properties observable and methods actions.

   ```TS
   import { makeAutoObservable } from "mobx";

   class TimerStore {
     secondsPassed = 0;
     constructor() {
       makeAutoObservable(this); // Automatically tracks state and actions
     }
     increase() {
       this.secondsPassed += 1;
     }
   }
   const myTimer = new TimerStore();
   export default myTimer;
   ```

3. **Create Actions to Update State**:
> Defining methods within the store (actions) that modify the state. In makeAutoObservable, these are automatically treated as actions, which is best practice for modifying state.

4. **Make React Components Reactive**:
> Wrapping the React components with the observer function from mobx-react-lite. This ensures the component re-renders whenever the data it uses in the store changes.

   ```TS
   import { observer } from "mobx-react-lite";
   import myTimer from "./TimerStore";

   const TimerView = observer(({ timer }) => (
     <button onClick={() => timer.increase()}>
       Seconds passed: {timer.secondsPassed}
     </button>
   ));
   ```

5. **Use the Store in the Component Tree**:
> Passing the store instance to the relevant components, usually via props or a Context Provider. 

**Core Concepts Summary**:
* __Observable__: Data that can be tracked.
* __Action__: Functions that change state.
* __Observer__: Components that update when observables change.
* __Computed__: Values derived from state, cached automatically.