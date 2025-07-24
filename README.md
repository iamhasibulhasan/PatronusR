PatronusR
=======
[![NuGet](https://img.shields.io/nuget/v/patronusr.svg)](https://www.nuget.org/packages/patronusr)

Simple patronusr implementation in .NET

Supports request/response, commands, queries via C# generic variance.

### Installing PatronusR

You should install [PatronusR with NuGet](https://www.nuget.org/packages/patronusr):

    Install-Package PatronusR
    
Or via the .NET Core command line interface:

    dotnet add package PatronusR

### Registering with `IServiceCollection`

PatronusR supports `Microsoft.Extensions.DependencyInjection.Abstractions` directly. To register various PatronusR services and handlers:

```
services.AddPatronusR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());
```

or with an assembly:

```
services.AddPatronusR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
```

This registers:

- `IPatronusR` as Scoped
- `ISender` as Scoped

To register behaviors, stream behaviors, pre/post processors:

```csharp
services.AddPatronusR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly);
    });
```