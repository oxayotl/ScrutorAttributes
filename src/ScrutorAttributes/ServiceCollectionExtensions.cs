using Microsoft.Extensions.DependencyModel;
using Scrutor;
using ScrutorAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public enum RegisterAs {
    Everything,
    Self,
    Interfaces,
    OneInterfaceOrSelf,
    InterfacesFromSameAssembly,
    SelfAndInterfacesFromSameAssembly
}

/// <summary>
/// Provides extension methods for IServiceCollection to enable advanced dependency injection scanning.
/// </summary>
public static class ServiceCollectionExtensions {

    private static DependencyContext FindDependencyContext(DependencyContext? providedContext = null) {
        return providedContext
            ?? DependencyContext.Default
            ?? throw new Exception("Unable to get the default context. This likely means you are running a single file application. Please provide explicitely the dependency context when calling AddScrutorAttributes");
    }

    private static RegisterAs DefaultInjectionStrategyWhenUnspecified => RegisterAs.Everything;

    /// <summary>
    /// Scans and registers services from all assemblies in the dependency context that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributes(this IServiceCollection services, DependencyContext? context = null) {
        return AddScrutorAttributes(services, DefaultInjectionStrategyWhenUnspecified, context);
    }

    /// <summary>
    /// Scans and registers services from all assemblies in the dependency context that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="defaultInjectionStrategy">Which strategy to use to determine under which type a class will be registered if none is explicitely specified by the attribute.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributes(this IServiceCollection services, RegisterAs defaultInjectionStrategy, DependencyContext? context = null) {
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context))
        .AddClassesFromAnnotations(defaultInjectionStrategy));

        return services;
    }

    /// <summary>
    /// Scans and registers services from assemblies matching the predicate that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="predicate">A predicate to filter which assemblies to scan.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributes(this IServiceCollection services, Func<Assembly, bool> predicate, DependencyContext? context = null) {
        return AddScrutorAttributes(services, predicate, DefaultInjectionStrategyWhenUnspecified, context);
    }

    /// <summary>
    /// Scans and registers services from assemblies matching the predicate that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="predicate">A predicate to filter which assemblies to scan.</param>
    /// <param name="defaultInjectionStrategy">Which strategy to use to determine under which type a class will be registered if none is explicitely specified by the attribute.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributes(this IServiceCollection services, Func<Assembly, bool> predicate, RegisterAs defaultInjectionStrategy, DependencyContext? context = null) {
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context), predicate)
        .AddClassesFromAnnotations(defaultInjectionStrategy));

        return services;
    }

    /// <summary>
    /// Scans and registers services from assemblies whose names start with the specified prefix.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="prefix">The prefix that assembly names must start with.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributesForAssembliesStartingWith(this IServiceCollection services, string prefix, DependencyContext? context = null) {
        return AddScrutorAttributesForAssembliesStartingWith(services, prefix, DefaultInjectionStrategyWhenUnspecified, context);
    }

    /// <summary>
    /// Scans and registers services from assemblies whose names start with the specified prefix.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="prefix">The prefix that assembly names must start with.</param>
    /// <param name="defaultInjectionStrategy">Which strategy to use to determine under which type a class will be registered if none is explicitely specified by the attribute.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributesForAssembliesStartingWith(this IServiceCollection services, string prefix, RegisterAs defaultInjectionStrategy, DependencyContext? context = null) {
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context), assembly =>
            assembly.FullName != null && assembly.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        .AddClassesFromAnnotations(defaultInjectionStrategy));

        return services;
    }

    /// <summary>
    /// Scans and registers services from assemblies whose names contain the specified text.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="text">The text that assembly names must contain.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributesForAssembliesContaining(this IServiceCollection services, string text, DependencyContext? context = null) {
        return AddScrutorAttributesForAssembliesContaining(services, text, DefaultInjectionStrategyWhenUnspecified, context);
    }

    /// <summary>
    /// Scans and registers services from assemblies whose names contain the specified text.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="text">The text that assembly names must contain.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributesForAssembliesContaining(this IServiceCollection services, string text, RegisterAs defaultInjectionStrategy, DependencyContext? context = null) {
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context), assembly =>
            assembly.FullName != null && assembly.FullName.Contains(text, StringComparison.OrdinalIgnoreCase))
        .AddClassesFromAnnotations(defaultInjectionStrategy));

        return services;
    }

    /// <summary>
    /// Scans and registers services from the calling assembly that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributesForThisAssembly(this IServiceCollection services) {
        var callingAssembly = Assembly.GetCallingAssembly();
        services.Scan(scan => scan
        .FromAssemblies(callingAssembly)
        .AddClassesFromAnnotations(DefaultInjectionStrategyWhenUnspecified));

        return services;
    }

    /// <summary>
    /// Scans and registers services from the calling assembly that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributesForThisAssembly(this IServiceCollection services, RegisterAs defaultInjectionStrategy) {
        var callingAssembly = Assembly.GetCallingAssembly();
        services.Scan(scan => scan
        .FromAssemblies(callingAssembly)
        .AddClassesFromAnnotations(defaultInjectionStrategy));

        return services;
    }

    /// <summary>
    /// Configures service registration based on lifetime marker interfaces.
    /// Scans for classes implementing ISingletonLifetime, ITransientLifetime, IScopedLifetime, and their self-registration variants.
    /// </summary>
    /// <param name="selector">The implementation type selector to configure.</param>
    /// <returns>The configured implementation type selector.</returns>
    private static IImplementationTypeSelector AddClassesFromAnnotations(this IImplementationTypeSelector selector, RegisterAs defaultInjectionStrategy = RegisterAs.Everything) {
        selector.AddClasses(classes => classes.
            WithAttribute<Injected>()
        )
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .As(t => {
            if (t.IsGenericType) {
                return [];
            }
            var injectedAttribute = (t.GetCustomAttributes(typeof(Injected), true).First() as Injected)!;

            if (injectedAttribute.If != null) {
                if (Environment.GetEnvironmentVariable(injectedAttribute.If) == null && injectedAttribute.NotEqual == null) {
                    return [];
                }
                if (injectedAttribute.Equal != null && !string.Equals(Environment.GetEnvironmentVariable(injectedAttribute.If), injectedAttribute.Equal, StringComparison.InvariantCultureIgnoreCase)) {
                    return [];
                }
                if (injectedAttribute.NotEqual != null && string.Equals(Environment.GetEnvironmentVariable(injectedAttribute.If), injectedAttribute.NotEqual, StringComparison.InvariantCultureIgnoreCase)) {
                    return [];
                }
            }
            if (injectedAttribute.IfNot != null && Environment.GetEnvironmentVariable(injectedAttribute.IfNot) != null) {
                return [];
            }


            if (injectedAttribute.AsSelf) {
                return [t];
            }
            List<Type> result = [.. injectedAttribute.InjectedAs];
            if (injectedAttribute.As != null) {
                result.Add(injectedAttribute.As);
            }
            if (result.Count == 0) {
                switch (defaultInjectionStrategy) {
                    case RegisterAs.Everything:
                    result.AddRange([.. t.GetInterfaces(), t]);
                    break;

                    case RegisterAs.Self:
                    result.Add(t);
                    break;

                    case RegisterAs.Interfaces:
                    result.AddRange([.. t.GetInterfaces()]);
                    break;

                    case RegisterAs.OneInterfaceOrSelf:
                    var implementedInterfaces = t.GetInterfaces();
                    result.Add(implementedInterfaces.Length == 1 ? implementedInterfaces[0] : t);
                    break;

                    case RegisterAs.InterfacesFromSameAssembly:
                    result.AddRange(t.GetInterfaces().Where(i => i.Assembly == t.Assembly));
                    break;

                    case RegisterAs.SelfAndInterfacesFromSameAssembly:
                    result.AddRange([.. t.GetInterfaces().Where(i => i.Assembly == t.Assembly), t]);
                    break;
                }
            }
            return result;
        }
        )
        .WithLifetime(t => {
            var injectedAttribute = (t.GetCustomAttributes(typeof(Injected), true).First() as Injected)!;
            return injectedAttribute.Lifetime;
        }
        );
        return selector;
    }
}
