using ScrutorAttributes.Test.Assembly;

namespace ScrutorAttributes.Tests.TestServices;

// Singleton Services
public interface ISingletonService {
    string GetMessage();
}

[InjectedSingleton(As =typeof(ISingletonService))]
public class SingletonService : ISingletonService {
    private readonly Guid _id = Guid.NewGuid();

    public string GetMessage() => $"Singleton: {_id}";
}

// Self Singleton Services
[InjectedSingleton]
public class SelfSingletonService {
    private readonly Guid _id = Guid.NewGuid();

    public string GetMessage() => $"SelfSingleton: {_id}";
}

// Transient Services
public interface ITransientService {
    string GetMessage();
}
[InjectedTransient(As =typeof(ITransientService))]
public class TransientService : ITransientService {
    private readonly Guid _id = Guid.NewGuid();

    public string GetMessage() => $"Transient: {_id}";
}

// Self Transient Services
[InjectedTransient]
public class SelfTransientService {
    private readonly Guid _id = Guid.NewGuid();

    public string GetMessage() => $"SelfTransient: {_id}";
}

// Scoped Services
public interface IScopedService{
    string GetMessage();
}

[InjectedScoped(As = typeof(IScopedService))]
public class ScopedService : IScopedService {
    private readonly Guid _id = Guid.NewGuid();

    public string GetMessage() => $"Scoped: {_id}";
}

// Self Scoped Services
[InjectedScoped]
public class SelfScopedService {
    private readonly Guid _id = Guid.NewGuid();

    public string GetMessage() => $"SelfScoped: {_id}";
}

// Multiple Implementations
public interface IMultipleImplementationService {
    string GetName();
}

public class FirstImplementation : IMultipleImplementationService {
    public string GetName() => "First";
}

public class SecondImplementation : IMultipleImplementationService {
    public string GetName() => "Second";
}

// Service with Dependencies
public interface IComplexService {
    string ProcessData();
}

[InjectedScoped(As = typeof(IComplexService))]
public class ComplexService : IComplexService {
    private readonly ISingletonService _singletonService;
    private readonly ITransientService _transientService;

    public ComplexService(ISingletonService singletonService, ITransientService transientService) {
        _singletonService = singletonService;
        _transientService = transientService;
    }

    public string ProcessData() => $"Complex: {_singletonService.GetMessage()} + {_transientService.GetMessage()}";
}

// Abstract class test
[InjectedSingleton]
public abstract class BaseService {
    public abstract string GetData();
}

public class ConcreteService : BaseService {
    public override string GetData() => "Concrete Data";
}

// Generic service test
public interface IGenericService<T> {
    T GetDefault();
}

[InjectedTransient]
public class GenericService<T> : IGenericService<T> {
    public T GetDefault() => default!;
}

public class IntGenericService : GenericService<int> {}

[InjectedSingleton(If = "some_environement_variable")]
public class IfService {}

public static class TestEnvironementVariable {
    public const string Name = "some_environement_variable";
    public const string SameValue = "same_value";
    public const string DifferentValue = "different_value";
    public const string SameValueWithDifferentCase = "sAmE_VaLuE";
}

[InjectedSingleton(IfNot = TestEnvironementVariable.Name)]
public class IfNotService {}


[InjectedSingleton(If = TestEnvironementVariable.Name, Equal = TestEnvironementVariable.SameValue)]
public class IfEqualService {}

[InjectedSingleton(If = TestEnvironementVariable.Name, NotEqual = TestEnvironementVariable.SameValue)]
public class IfNotEqualService { }


[InjectedSingleton]
public class InterfaceImplementation : ILibraryInterface { }
