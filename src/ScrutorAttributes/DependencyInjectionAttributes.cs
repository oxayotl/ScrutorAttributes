using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace ScrutorAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class InjectedAttribute(ServiceLifetime serviceLifetime) : Attribute {
    public ServiceLifetime Lifetime { get; set; } = serviceLifetime;

    /// <value>A list of <c>Type</c> that the class will be registered as.</value>
    public Type[] InjectedAs { get; set; } = [];
    /// <value>A <c>Type</c> that the class will be registered as.</value>
    public Type? As { get; set; }
}

/// <summary>
/// Mark the class to tell ScrutorAttribute that it must be registered so it can be injected with a Singleton lifescope
/// </summary>
public class InjectedSingletonAttribute() : InjectedAttribute(ServiceLifetime.Singleton) { }
/// <summary>
/// Mark the class to tell ScrutorAttribute that it must be registered so it can be injected with a Scoped lifescope
/// </summary>
public class InjectedScopedAttribute() : InjectedAttribute(ServiceLifetime.Scoped) { }
/// <summary>
/// Mark the class to tell ScrutorAttribute that it must be registered so it can be injected with a Transient lifescope
/// </summary>
public class InjectedTransientAttribute() : InjectedAttribute(ServiceLifetime.Transient) { }

public abstract class RegistrationConditionAttribute : Attribute {
    public abstract bool IsSatisfied();
}

/// <summary>
/// Make injection conditional on a specific environement variable being defined
/// </summary>
/// <param name="variableThatMustBeDefined">The name of the variable that must be defined</param>
/// <remarks>The injection will only happen if the class is also marked with a ScrutorAttribute lifetime attribute like <c>InjectedSingleton</c>, <c>InjectedScoped</c> or <c>InjectedTransient</c></remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class InjectIfDefinedAttribute(string variableThatMustBeDefined) : RegistrationConditionAttribute {
    public string VariableThatMustBeDefined { get; init; } = variableThatMustBeDefined;

    public override bool IsSatisfied() {
        return Environment.GetEnvironmentVariable(VariableThatMustBeDefined) != null;
    }
}

/// <summary>
/// Make injection conditional on a specific environement variable not being defined
/// </summary>
/// <param name="variableThatMustNotBeDefined">The name of the variable that must be undefined</param>
/// <remarks>The injection will only happen if the class is also marked with a ScrutorAttribute lifetime attribute like <c>InjectedSingleton</c>, <c>InjectedScoped</c> or <c>InjectedTransient</c></remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class InjectIfNotDefinedAttribute(string variableThatMustNotBeDefined) : RegistrationConditionAttribute {
    public string VariableThatMustBeDefined { get; init; } = variableThatMustNotBeDefined;

    public override bool IsSatisfied() {
        return Environment.GetEnvironmentVariable(VariableThatMustBeDefined) == null;
    }
}

/// <summary>
/// Make injection conditional on a specific environement variable being defined and equal to a set value
/// </summary>
/// <param name="variableToCheck">The name of the variable that must be checked</param>
/// <param name="valueToCheckAgainst">The value that the variable must be equal to for the class to be injected</param>
/// <remarks>The injection will only happen if the class is also marked with a ScrutorAttribute lifetime attribute like <c>InjectedSingleton</c>, <c>InjectedScoped</c> or <c>InjectedTransient</c></remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class InjectIfEqualAttribute(string variableToCheck, string valueToCheckAgainst) : RegistrationConditionAttribute {
    public string VariableToCheck { get; init; } = variableToCheck;
    public string ValueToCheckAgainst { get; init; } = valueToCheckAgainst;
    public StringComparison StringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;

    public override bool IsSatisfied() {
        return string.Equals(Environment.GetEnvironmentVariable(VariableToCheck), ValueToCheckAgainst, StringComparison);
    }
}

/// <summary>
/// Make injection conditional on a specific environement variable not being equal to some values
/// </summary>
/// <param name="variableToCheck">The name of the variable that must be checked</param>
/// <param name="valuesToCheckAgainst">The values that the variable must not be equal to for the class to be injected</param>
/// <remarks>The injection will only happen if the class is also marked with a ScrutorAttribute lifetime attribute like <c>InjectedSingleton</c>, <c>InjectedScoped</c> or <c>InjectedTransient</c></remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class InjectIfNotEqualAttribute(string variableToCheck, params string[] valuesToCheckAgainst) : RegistrationConditionAttribute {
    public string VariableToCheck { get; init; } = variableToCheck;
    public string[] ValuesToCheckAgainst { get; init; } = valuesToCheckAgainst;
    public StringComparison StringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;

    public override bool IsSatisfied() {
        return !ValuesToCheckAgainst.Any(v=>string.Equals(Environment.GetEnvironmentVariable(VariableToCheck), v, StringComparison));
    }
}

