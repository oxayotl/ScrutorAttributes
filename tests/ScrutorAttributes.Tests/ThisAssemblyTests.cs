using ScrutorAttributes.Tests.TestServices;
using System.Reflection;

namespace ScrutorAttributes.Tests;

/// <summary>
/// Tests for AddScrutorAttributesForThisAssembly method that scans only the calling assembly.
/// </summary>
[TestFixture]
public class ThisAssemblyTests {
    [Test]
    public void AddScrutorAttributesForThisAssembly_ShouldRegisterServicesFromCallingAssembly() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForThisAssembly();
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Services from test assembly should be registered
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<ITransientService>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<IScopedService>(), Is.Not.Null);
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_ShouldRegisterAllLifetimeInterfaces() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForThisAssembly();
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Verify all 6 lifetime interfaces are working
        using var scope = serviceProvider.CreateScope();

        Assert.That(scope.ServiceProvider.GetService<ISingletonService>(), Is.Not.Null, "ISingletonLifetime should work");
        Assert.That(scope.ServiceProvider.GetService<SelfSingletonService>(), Is.Not.Null, "ISelfSingletonLifetime should work");
        Assert.That(scope.ServiceProvider.GetService<ITransientService>(), Is.Not.Null, "ITransientLifetime should work");
        Assert.That(scope.ServiceProvider.GetService<SelfTransientService>(), Is.Not.Null, "ISelfTransientLifetime should work");
        Assert.That(scope.ServiceProvider.GetService<IScopedService>(), Is.Not.Null, "IScopedLifetime should work");
        Assert.That(scope.ServiceProvider.GetService<SelfScopedService>(), Is.Not.Null, "ISelfScopedLifetime should work");
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_ShouldReturnServiceCollection_ForChaining() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddScrutorAttributesForThisAssembly();

        // Assert
        Assert.That(result, Is.SameAs(services), "Should return the same ServiceCollection for method chaining");
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_CalledMultipleTimes_ShouldSkipDuplicates() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForThisAssembly();
        services.AddScrutorAttributesForThisAssembly();

        // Assert - Should only have one registration due to Skip strategy
        var singletonServices = services.Where(s => s.ServiceType == typeof(ISingletonService)).ToList();
        Assert.That(singletonServices.Count, Is.EqualTo(1), "Should not register duplicate services");
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_VerifyLifetimes_SingletonShouldBeSameInstance() {
        // Arrange
        var services = new ServiceCollection();
        services.AddScrutorAttributesForThisAssembly();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var instance1 = serviceProvider.GetService<ISingletonService>();
        var instance2 = serviceProvider.GetService<ISingletonService>();

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2), "Singleton should return the same instance");
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_VerifyLifetimes_TransientShouldBeDifferentInstances() {
        // Arrange
        var services = new ServiceCollection();
        services.AddScrutorAttributesForThisAssembly();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var instance1 = serviceProvider.GetService<ITransientService>();
        var instance2 = serviceProvider.GetService<ITransientService>();

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.Not.SameAs(instance2), "Transient should return different instances");
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_VerifyLifetimes_ScopedShouldBeSameWithinScope() {
        // Arrange
        var services = new ServiceCollection();
        services.AddScrutorAttributesForThisAssembly();
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        using (var scope1 = serviceProvider.CreateScope()) {
            var instance1 = scope1.ServiceProvider.GetService<IScopedService>();
            var instance2 = scope1.ServiceProvider.GetService<IScopedService>();

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance1, Is.SameAs(instance2), "Scoped should return same instance within scope");
        }

        using (var scope2 = serviceProvider.CreateScope()) {
            var instance3 = scope2.ServiceProvider.GetService<IScopedService>();

            using (var scope1 = serviceProvider.CreateScope()) {
                var instance1 = scope1.ServiceProvider.GetService<IScopedService>();
                Assert.That(instance3, Is.Not.SameAs(instance1), "Scoped should return different instances across scopes");
            }
        }
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_SelfRegistration_ShouldWorkForAllLifetimes() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForThisAssembly();
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Self-registration should work (no interface, registered as concrete type)
        Assert.That(serviceProvider.GetService<SelfSingletonService>(), Is.Not.Null, "Self singleton should be registered");
        Assert.That(serviceProvider.GetService<SelfTransientService>(), Is.Not.Null, "Self transient should be registered");
        Assert.That(serviceProvider.GetService<SelfScopedService>(), Is.Not.Null, "Self scoped should be registered");
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_ComplexServiceWithDependencies_ShouldResolve() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForThisAssembly();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var complexService = serviceProvider.GetService<IComplexService>();
        Assert.That(complexService, Is.Not.Null, "Complex service with dependencies should be resolved");

        var result = complexService!.ProcessData();
        Assert.That(result, Does.Contain("Complex:"), "Complex service should work correctly");
    }

    [Test]
    public void AddScrutorAttributesForThisAssembly_OnlyScansCallingAssembly() {
        // Arrange
        var services = new ServiceCollection();
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        // Act
        services.AddScrutorAttributesForThisAssembly();

        // Assert - Verify only services from this assembly are registered
        // Services from test assembly should be there
        var serviceProvider = services.BuildServiceProvider();
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null, $"Services from {testAssemblyName} should be registered");

        // Verify that it doesn't register services from all assemblies (less than AddScrutorAttributes would)
        var serviceCount = services.Count;
        var servicesWithFullScan = new ServiceCollection();
        servicesWithFullScan.AddScrutorAttributes();
        var fullScanCount = servicesWithFullScan.Count;

        Assert.That(serviceCount, Is.LessThanOrEqualTo(fullScanCount),
            "AddScrutorAttributesForThisAssembly should register fewer or equal services than AddScrutorAttributes");
    }
}
