# EasyScrutor Examples

This directory contains example projects demonstrating how to use **EasyScrutor** for automatic dependency injection registration in different ASP.NET Core application types.

## What is EasyScrutor?

EasyScrutor is a dependency injection helper package that automatically scans and registers services based on marker interfaces, eliminating the need for manual service registration in `Program.cs`.

## How It Works

Instead of manually registering services like this:
```csharp
builder.Services.AddScoped<IMyService, MyService>();
builder.Services.AddSingleton<IAnotherService, AnotherService>();
```

You simply:
1. Add Attributes to on your services (`InjectedScoped`, `InjectedSingleton`, `InjectedTransient`)
2. Call `AddScrutorAttributes()` once
3. All services are automatically discovered and registered!

## Available Examples

### 1. WebApiExample
A minimal Web API demonstrating:
- Auto-registration of scoped and singleton services
- Service injection in minimal API endpoints
- Custom endpoint using injected services

**Run:** `dotnet run --project WebApiExample`
**Test:** Navigate to `/greeting/YourName` to see auto-registered services in action

### 2. MvcExample
An MVC application showing:
- Auto-registration of transient services
- Service injection in MVC controllers
- Using services in views

**Run:** `dotnet run --project MvcExample`
**View:** Home page displays message from auto-registered service

### 3. BlazorServerExample
A Blazor Server app demonstrating:
- Auto-registration of singleton services
- Service injection in Blazor components
- Shared state across users

**Run:** `dotnet run --project BlazorServerExample`
**Try:** Counter page uses a singleton service shared across all sessions

### 4. ConsoleHostExample
A generic host worker service (console application) demonstrating:
- Auto-registration in non-web applications
- Background worker using auto-registered services
- Singleton metrics collector shared across the application
- Scoped data processor

**Run:** `dotnet run --project ConsoleHostExample`
**See:** Console output showing auto-registered services in action

## Usage Pattern

### Step 1: Create Your Service Interface and Implementation

```csharp
// Interface
public interface IMyService
{
    string DoSomething();
}

// Implementation - Add the appropriate lifetime marker attribute
[InjectedScoped]
public class MyService : IMyService
{
    public string DoSomething() => "Hello from auto-registered service!";
}
```

### Step 2: Register Services in Program.cs

**For Web Applications (API, MVC, Blazor):**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add all your other services...
builder.Services.AddControllers();

// Add ScrutorAttributes - This scans and registers all services
builder.Services.AddScrutorAttributes();

var app = builder.Build();

app.Run();
```

**For Generic Host / Console Applications:**
```csharp
var builder = Host.CreateApplicationBuilder(args);

// Add ScrutorAttributes - This scans and registers all services
builder.Services.AddScrutorAttributes();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
```

### Step 3: Use Your Services Anywhere

```csharp
public class MyController : Controller
{
    private readonly IMyService _myService;

    public MyController(IMyService myService)
    {
        _myService = myService; // Automatically injected!
    }
}
```

## Lifetime Marker Interfaces

| Interface | Lifetime | Use Case |
|-----------|----------|----------|
| `InjectedScoped` | Scoped | Services that should live for the duration of a request |
| `InjectedSingleton` | Singleton | Services that should be created once and shared across the app |
| `InjectedTransient` | Transient | Services that should be created new each time they're requested |

## Benefits

✅ **Cleaner Code** - No more cluttered Program.cs with dozens of service registrations  
✅ **Convention-Based** - Simply add an attribute to define the lifetime  
✅ **Type-Safe** - Compile-time checked, no magic strings  
✅ **Maintainable** - Services declare their own lifetime alongside their implementation  
✅ **Flexible** - Can still manually register services when needed  
✅ **Discoverable** - Easy to find all services by searching for lifetime attribute

## Building and Running

Build all examples:
```bash
dotnet build
```

Run a specific example:
```bash
dotnet run --project WebApiExample
dotnet run --project MvcExample
dotnet run --project BlazorServerExample
dotnet run --project ConsoleHostExample
```

## Learn More

For more information, visit the [ScrutorAttribute GitHub repository](https://github.com/oxayotl/ScrutorAttributes).

## Troubleshooting

### Services Not Being Registered?

1. **Check the namespace**: Services must be in a namespace that's scanned by `AddScrutorAttributes()`
2. **Verify attribute presence**: Ensure your service is marked with one of the lifetime attribute
3. **Public classes only**: Services must be public to be discovered
4. **Check assembly**: By default, the entry assembly is scanned

### Need Help?

- Check the [examples](https://github.com/oxayotl/ScrutorAttributes/tree/master/examples) for working code
- Open an [issue](https://github.com/oxayotl/ScrutorAttributes/issues) if you find a bug
- Start a [discussion](https://github.com/oxayotl/ScrutorAttributes/discussions) for questions
