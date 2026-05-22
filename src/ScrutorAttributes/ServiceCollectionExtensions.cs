using Microsoft.Extensions.DependencyModel;
using Scrutor;
using ScrutorAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for IServiceCollection to enable advanced dependency injection scanning.
/// </summary>
public static class ServiceCollectionExtensions {

    private static DependencyContext FindDependencyContext(DependencyContext? providedContext = null) {
        return providedContext
            ?? DependencyContext.Default
            ?? throw new Exception("Unable to get the default context. This likely means you are running a single file application. Please provide explicitely the dependency context when calling AddScrutorAttributes");
    }


    /// <summary>
    /// Scans and registers services from all assemblies in the dependency context that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributes(this IServiceCollection services, DependencyContext? context = null) {
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context))
        .AddClassesFromAnnotations());

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
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context), predicate)
        .AddClassesFromAnnotations());

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
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context), assembly =>
            assembly.FullName != null && assembly.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        .AddClassesFromAnnotations());

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
        services.Scan(scan => scan
        .FromDependencyContext(FindDependencyContext(context), assembly =>
            assembly.FullName != null && assembly.FullName.Contains(text, StringComparison.OrdinalIgnoreCase))
        .AddClassesFromAnnotations());

        return services;
    }

    /// <summary>
    /// Scans and registers services from the calling assembly that implement lifetime marker interfaces.
    /// Services are registered based on their implemented lifetime interfaces (ISingletonLifetime, ITransientLifetime, IScopedLifetime).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="context">Optional parameter to explicitely provide a DependencyContext, for instance for single file applications where DependencyContext.Default is null.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScrutorAttributesForThisAssembly(this IServiceCollection services, DependencyContext? context = null) {
        var callingAssembly = Assembly.GetCallingAssembly();
        services.Scan(scan => scan
        .FromAssemblies(callingAssembly)
        .AddClassesFromAnnotations());

        return services;
    }

    /// <summary>
    /// Configures service registration based on lifetime marker interfaces.
    /// Scans for classes implementing ISingletonLifetime, ITransientLifetime, IScopedLifetime, and their self-registration variants.
    /// </summary>
    /// <param name="selector">The implementation type selector to configure.</param>
    /// <returns>The configured implementation type selector.</returns>
    private static IImplementationTypeSelector AddClassesFromAnnotations(this IImplementationTypeSelector selector) {
    selector.AddClasses(classes => classes.
        WithAttribute<Injected>()
    )
    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
    .As(t => {
        if (t.IsGenericType) {
            return [];
        }
        var injectedAttribute = (t.GetCustomAttributes(typeof(Injected), true).First() as Injected)!;
        if (injectedAttribute.AsSelf) {
            return [t];
        }
        List<Type> result = [.. injectedAttribute.InjectedAs];
        if (injectedAttribute.As != null) {
            result.Add(injectedAttribute.As);
        }
        if (result.Count == 0) {
            var implementedInterfaces = t.GetInterfaces();
            result.Add(implementedInterfaces.Length == 1 ? implementedInterfaces[0] : t);
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
