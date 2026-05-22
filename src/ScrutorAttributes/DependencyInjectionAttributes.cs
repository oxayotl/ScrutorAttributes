using Microsoft.Extensions.DependencyInjection;
using System;
namespace ScrutorAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class Injected(ServiceLifetime serviceLifetime, bool asSelf = false) : Attribute {
    public ServiceLifetime Lifetime { get; set; } = serviceLifetime;
    public Type[] InjectedAs { get; set; } = [];
    public Type? As { get; set; }
    public bool AsSelf { get; set; } = asSelf;

    /// <summary>
    /// Make injection conditional on a specific environement variable being defined
    /// </summary>
    /// <value>The name of the variable that must be defined</value>
    /// <remarks>Warning : When used in combination with the <c>NotEqual</c> parameter, injection will still happen
    /// if the variable is not set.</remarks>
    public string? If { get; set; }
    /// <summary>
    /// Make injection conditional on a specific environement variable not being defined
    /// </summary>
    /// <value>The name of the variable that must be undefined</value>
    public string? IfNot { get; set; }
    /// <summary>
    /// Make injection conditional on a specific environement variable being defined to a specific value
    /// </summary>
    /// <value>The value that the variable must be defined to</value>
    /// <remarks>The name of the variable is taken from the <c>If</c> parameter. When <c>If</c> is not set,
    /// this parameter will be ignored.</remarks>
    public string? Equal { get; set; }
    /// <summary>
    /// Make injection conditional on a specific environement variable not being defined to a specific value
    /// </summary>
    /// <value>The value that the variable must not be defined to</value>
    /// <remarks>The name of the variable is taken from the <c>If</c> parameter. When <c>If</c> is not set,
    /// no injection will happen.</remarks>
    public string? NotEqual { get; set; }
}

public class InjectedSingleton() : Injected(ServiceLifetime.Singleton) { }
public class InjectedScoped() : Injected(ServiceLifetime.Scoped) { }
public class InjectedTransient() : Injected(ServiceLifetime.Transient) { }
public class InjectedSelfSingleton() : Injected(ServiceLifetime.Singleton, true) { }
public class InjectedSelfScoped() : Injected(ServiceLifetime.Scoped, true) { }
public class InjectedSelfTransient() : Injected(ServiceLifetime.Transient, true) { }
