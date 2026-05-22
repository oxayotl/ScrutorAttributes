using Microsoft.Extensions.DependencyInjection;
using System;
namespace ScrutorAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class Injected(ServiceLifetime serviceLifetime, bool asSelf = false) : Attribute
{
    public ServiceLifetime Lifetime { get; set; } = serviceLifetime;
    public Type[] InjectedAs { get; set; } = [];
    public Type? As { get; set; }
    public bool AsSelf { get; set; } = asSelf;
}

public class InjectedSingleton() : Injected(ServiceLifetime.Singleton) { }
public class InjectedScoped() : Injected(ServiceLifetime.Scoped) { }
public class InjectedTransient() : Injected(ServiceLifetime.Transient) { }
public class InjectedSelfSingleton() : Injected(ServiceLifetime.Singleton, true) { }
public class InjectedSelfScoped() : Injected(ServiceLifetime.Scoped, true) { }
public class InjectedSelfTransient() : Injected(ServiceLifetime.Transient, true) { }
