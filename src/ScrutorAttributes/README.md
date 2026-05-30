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

## Registration strategies

### Set which assemblies you want to scan for registration

** Scan every assemblies with **

```csharp
builder.Services.AddEasyScrutor();
```

**Scan assemblies with a specific prefix**

```csharp
builder.Services.AddScrutorAttributesForAssembliesStartingWith("some.prefix");
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
Func<Assembly, bool> predicate = ...
builder.Services.AddScrutorAttributes(predicate);
```

### Select a strategy to determine which type each Attribute class should be registered as

**Register each class as itself with**

```csharp
Func<Assembly, bool> predicate = ...
builder.Services.AddScrutorAttributes(RegisterAs.Self);
```


**Register each class as every interfaces it implements with**

```csharp
Func<Assembly, bool> predicate = ...
builder.Services.AddScrutorAttributes(RegisterAs.Interfaces);
```


**Register each class as the interface it implements if there is exactly one such interface, or as itself otherwise, with**

```csharp
Func<Assembly, bool> predicate = ...
builder.Services.AddScrutorAttributes(RegisterAs.OneInterfaceOrSelf);
```


**Register each class as every interface it implements and that is part of the same assembly**

```csharp
Func<Assembly, bool> predicate = ...
builder.Services.AddScrutorAttributes(RegisterAs.InterfacesFromSameAssembly);
```


**Register each class as every interface it implements and that is part of the same assembly, and also as itself**

```csharp
Func<Assembly, bool> predicate = ...
builder.Services.AddScrutorAttributes(RegisterAs.SelfAndInterfacesFromSameAssembly);
```


**Or just register each class as itself and every interface it implements, which is the default behaviour**

```csharp
Func<Assembly, bool> predicate = ...
builder.Services.AddScrutorAttributes(RegisterAs.Everything);
```



## Documentation

For examples, and advanced usage, visit the [GitHub repository](https://github.com/oxayotl/ScrutorAttributes).

## License

MIT
