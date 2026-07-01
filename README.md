## ScrutorAttributes

ASP.NET Core [Scrutor](https://github.com/khellang/Scrutor) extension for automatic registration of classes marked with Attribute `InjectedScoped`, `InjectedSingleton` and `InjectedTransient`

### About This Project

ScrutorAttributes is a fork of [EasyScrutor](https://github.com/alexdresko/EasyScrutor) project, inspired by Spring injection mechanism. It replaces the `interface`-based injection by `Attribute`-based injection.

### Build Status
| Build server    | Platform       | Status      |
|-----------------|----------------|-------------|
| NuGet           | Package        | [![NuGet](https://img.shields.io/nuget/v/oxayotl.ScrutorAttributes.svg)](https://www.nuget.org/packages/oxayotl.ScrutorAttributes/) |
| NuGet           | Downloads      | [![NuGet Downloads](https://img.shields.io/nuget/dt/oxayotl.ScrutorAttributes.svg)](https://www.nuget.org/packages/ScrutorAttributes/) |
| GitHub          | Release        | [![GitHub Release](https://img.shields.io/github/release/oxayotl/ScrutorAttributes.svg)](https://github.com/oxayotl/ScrutorAttributes/releases) |
| License         | MIT            | [![License](https://img.shields.io/github/license/oxayotl/ScrutorAttributes.svg)](LICENSE) |

## Quick Start

Get started with ScrutorAttributes in just 3 steps:

**1. Install the package:**
```bash
dotnet add package ScrutorAttributes
```

**2. Mark your services with a lifetime Attribute:**
```csharp
public interface IMyService { string GetMessage(); }
[InjectedScoped]
public class MyService : IMyService
{
    public string GetMessage() => "Hello from ScrutorAttributes!";
}
```

**3. Register in Program.cs:**
```csharp
builder.Services.AddScrutorAttributes();
```

That's it! Your services are now automatically registered and ready to inject anywhere in your application.

## Installation

Install the [ScrutorAttributes NuGet Package](https://www.nuget.org/packages/ScrutorAttributes).

### Package Manager Console

```
Install-Package ScrutorAttributes
```

### .NET Core CLI

```
dotnet add package ScrutorAttributes
```

## Usage

ScrutorAttributes automatically discovers and registers your services by scanning for classes marked with lifetime attributes.

### Step 1: Mark your service classes

Add one of the following Attributes to your service classes:
- `InjectedScoped` - Scoped registration
- `InjectedTransient` - Transient registration
- `InjectedSingleton` - Singleton registration

```csharp
using ScrutorAttributes;

public interface IDataService
{
    Task<string> GetDataAsync();
}

// This class will be automatically registered as Scoped
[InjectedScoped]
public class DataService : IDataService
{
    public async Task<string> GetDataAsync()
    {
        return await Task.FromResult("Hello from DataService!");
    }
}
```

### Step 2: Register ScrutorAttributes in your application

**ASP.NET Core:**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add EasyScrutor - automatically scans and registers services
builder.Services.AddScrutorAttributes();

var app = builder.Build();
app.Run();
```

**Console/Worker Service:**

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Add EasyScrutor - automatically scans and registers services
builder.Services.AddScrutorAttributes();

var host = builder.Build();
host.Run();
```

### Step 3: Use your services

Services are injected automatically through constructor injection:

```csharp
public class MyController : ControllerBase
{
    private readonly IDataService _dataService;

    public MyController(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<IActionResult> Get()
    {
        var data = await _dataService.GetDataAsync();
        return Ok(data);
    }
}
```

That's it! No manual service registration needed - ScrutorAttributes handles it all for you.

## Advanced Usage

### Filtering Assemblies for Performance

By default, `AddScrutorAttributes()` scans all assemblies in your application's dependency context. For better performance, especially in large applications, you can filter which assemblies to scan.

**Scan every assemblies with**

```csharp
builder.Services.AddScrutorAttributes();
```

**Scan assemblies with a specific prefix**

```csharp
builder.Services.AddScrutorAttributesForAssembliesStartingWith("some.prefix");
```

**Scan assemblies containing a specific string**

```csharp
builder.Services.AddScrutorAttributesForAssembliesContaining("some.string");
```

**Scan only from the assembly calling ScrutorAttributes with**

```csharp
builder.Services.AddScrutorAttributesForThisAssembly();
```

**Or use any custom function to select which assemblies to scan from with**

```csharp
builder.Services.AddScrutorAttributesForThisAssembly(assembly =>
{
    // Only scan assemblies from your application, exclude framework assemblies
    return !assembly.FullName?.StartsWith("Microsoft.", StringComparison.Ordinal) == true &&
           !assembly.FullName?.StartsWith("System.", StringComparison.Ordinal) == true &&
           !assembly.FullName?.StartsWith("netstandard", StringComparison.Ordinal) == true;
});
```

This can significantly improve application startup time by reducing the number of assemblies scanned for service registration.

### Select which type each Attribute class should be registered as

#### As a global strategy

By default, `ScrutorAttribute` will register each `class` as itself and every `interface` the class implements. You can make registration more selective by selecting one of the following strategies:

- **Register each class only as itself with**

```csharp
builder.Services.AddScrutorAttributes(RegisterAs.Self);
```

- **Register each class as every interfaces it implements with**

```csharp
builder.Services.AddScrutorAttributes(RegisterAs.Interfaces);
```

- **Register each class as the interface it implements if there is exactly one such interface, or as itself otherwise, with**

```csharp
builder.Services.AddScrutorAttributes(RegisterAs.OneInterfaceOrSelf);
```

- **Register each class as every interface it implements and that is part of the same assembly**

```csharp
builder.Services.AddScrutorAttributes(RegisterAs.InterfacesFromSameAssembly);
```

- **Register each class as every interface it implements and that is part of the same assembly, and also as itself**

```csharp
builder.Services.AddScrutorAttributes(RegisterAs.SelfAndInterfacesFromSameAssembly);
```

- **Or you can explicitely force the default option of registering each class as itself and every interface it implements with**

```csharp
builder.Services.AddScrutorAttributes(RegisterAs.Everything);
```

#### As a per-Attribute override

If you need to, you can override the global strategy, and explicitely specify which types a class should be injected at, by using the Attributes' parameters `As` (to specify one type the class should be registered as), or `InjectedAs` (to specify multiples types).

```csharp
using ScrutorAttributes;

public interface IDataService
{
    Task<string> GetDataAsync();
}

// This class will not be registered as IDisposable
[InjectedScoped(InjectedAs = [typeof(DataService), typeof(IDataService)])
public class DataService : IDataService, IDisposable
{
    public async Task<string> GetDataAsync()
    {
        return await Task.FromResult("Hello from DataService!");
    }
}
```

### Conditional registration

One key interest of dependency injection is to make it easier to have multiple implementation of an interface, and switch which implementation to use depending on context. To allow this with `ScrutorAttributes`, you can add the following Attributes to a class
 - `InjectIfDefined("variable_name")` to register the class only if a variable with a certain name is registered in the application's environement
 - `InjectIfNotDefined("variable_name")` to register the class only if no variable with a certain name are registered in the application's environement
 - `InjectIfEqual("variable_name", "variable_value")` to register the class only if the variable with name `variable_name` is defined and equal to `variable_value`. By default the comparision is done using `StringComparison.InvariantCultureIgnoreCase` but you can specify how the comparision should be made by using the optional parameter `StringComparision`
 - `InjectIfNotEqual("variable_name", "variable_value", ...)` to register the class only if the variable with name `variable_name` is not equal any of the following arguments. You can specify how the string comparision is made with `StringComparision` like with previous Attribute `InjectIfEqual`

 If multiple condition registration attributes are set, the class will only the registered if all of them are verified.

 #### Example

```csharp
using ScrutorAttributes;

[InjectedScoped, InjectIfDefined("use_data_service"), InjectIfNotEqual("type_data_service", "first_data_service_type", "second_data_service_type", StringComparison = StringComparison.InvariantCulture)])
public class DefaultDataService : IDataService
{
// ...
}
```
This class will be injected only if `Environement` contains a variable named `use_data_service`, AND either there is no defined `type_data_service` variable, or `type_data_service` is defined to a value that is neither `first_data_service_type` nor `second_data_service_type`, using a case-senstive, invariant culture string comparison.


## Examples

See [examples/README.md](examples/README.md) for runnable sample apps (Web API, MVC, Blazor Server, and a console/worker host).

## Contributing

We welcome contributions! Please read our [CONTRIBUTING.md](CONTRIBUTING.md) guide to get started.

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for the complete version history and detailed list of changes since forking from [Scrutor.AspNetCore](https://github.com/sefacan/Scrutor.AspNetCore).
