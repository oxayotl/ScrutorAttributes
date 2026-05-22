# ScrutorAttributes

Convention-based dependency injection for .NET using Scrutor. Automatically register your services by implementing simple marker attributes - no manual registration needed.

## Installation

```bash
dotnet add package ScrutorAttributes
```

## Quick Start

**1. Mark your services with lifetime attribute:**

```csharp
using ScrutorAttributes;

public interface IMyService { }

// Automatically registered as Scoped
[InjectedScoped(As = typeof(IMyService)]
public class MyService : IMyService { }
```

**2. Add to your application:**

```csharp
builder.Services.AddEasyScrutor();
```

**3. Use your services:**

```csharp
public class MyController
{
    public MyController(IMyService myService) { }
}
```

## Lifetime Interfaces

- `InjectedScoped` - Scoped registration
- `InjectedTransient` - Transient registration
- `InjectedSingleton` - Singleton registration

## Documentation

For complete documentation, examples, and advanced usage, visit the [GitHub repository](https://github.com/oxayotl/ScrutorAttributes).

## License

MIT
