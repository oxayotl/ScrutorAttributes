using ScrutorAttributes.Tests.TestServices;
using System.Reflection;

namespace ScrutorAttributes.Tests;

/// <summary>
/// Tests for assembly filtering methods like AddScrutorAttributesForAssembliesStartingWith and AddScrutorAttributesForAssembliesContaining.
/// </summary>
[TestFixture]
public class AssemblyFilterTests {
    [Test]
    public void AddScrutorAttributesForAssembliesStartingWith_MatchingPrefix_ShouldRegisterServices() {
        // Arrange
        var services = new ServiceCollection();
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var prefix = testAssemblyName!.Substring(0, 10); // Get first 10 characters as prefix

        // Act
        services.AddScrutorAttributesForAssembliesStartingWith(prefix);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<ITransientService>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<IScopedService>(), Is.Not.Null);
    }

    [Test]
    public void AddScrutorAttributesForAssembliesStartingWith_NonMatchingPrefix_ShouldNotRegisterServices() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForAssembliesStartingWith("NonExistentPrefix");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Null);
        Assert.That(serviceProvider.GetService<ITransientService>(), Is.Null);
        Assert.That(serviceProvider.GetService<IScopedService>(), Is.Null);
    }

    [Test]
    public void AddScrutorAttributesForAssembliesStartingWith_CaseInsensitive_ShouldRegisterServices() {
        // Arrange
        var services = new ServiceCollection();
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var prefix = testAssemblyName!.Substring(0, 10).ToLower(); // Lowercase prefix

        // Act
        services.AddScrutorAttributesForAssembliesStartingWith(prefix);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null, "Should be case-insensitive");
    }

    [Test]
    public void AddScrutorAttributesForAssembliesStartingWith_ShouldReturnServiceCollection_ForChaining() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddScrutorAttributesForAssembliesStartingWith("Test");

        // Assert
        Assert.That(result, Is.SameAs(services), "Should return the same ServiceCollection for method chaining");
    }

    [Test]
    public void AddScrutorAttributesForAssembliesContaining_MatchingText_ShouldRegisterServices() {
        // Arrange
        var services = new ServiceCollection();
        var containingText = "Scrutor"; // Should be part of the assembly name

        // Act
        services.AddScrutorAttributesForAssembliesContaining(containingText);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<ITransientService>(), Is.Not.Null);
        Assert.That(serviceProvider.GetService<IScopedService>(), Is.Not.Null);
    }

    [Test]
    public void AddScrutorAttributesForAssembliesContaining_NonMatchingText_ShouldNotRegisterServices() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForAssembliesContaining("NonExistentText");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Null);
        Assert.That(serviceProvider.GetService<ITransientService>(), Is.Null);
        Assert.That(serviceProvider.GetService<IScopedService>(), Is.Null);
    }

    [Test]
    public void AddScrutorAttributesForAssembliesContaining_CaseInsensitive_ShouldRegisterServices() {
        // Arrange
        var services = new ServiceCollection();
        var containingText = "SCRUTOR"; // Uppercase

        // Act
        services.AddScrutorAttributesForAssembliesContaining(containingText);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null, "Should be case-insensitive");
    }

    [Test]
    public void AddScrutorAttributesForAssembliesContaining_ShouldReturnServiceCollection_ForChaining() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddScrutorAttributesForAssembliesContaining("Test");

        // Assert
        Assert.That(result, Is.SameAs(services), "Should return the same ServiceCollection for method chaining");
    }

    [Test]
    public void AddScrutorAttributesForAssembliesContaining_PartialMatch_ShouldRegisterServices() {
        // Arrange
        var services = new ServiceCollection();
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        // Take middle part of assembly name
        var partialText = testAssemblyName!.Substring(5, 5);

        // Act
        services.AddScrutorAttributesForAssembliesContaining(partialText);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null, "Should match partial text");
    }

    [Test]
    public void AddScrutorAttributesForAssembliesStartingWith_CalledMultipleTimes_ShouldSkipDuplicates() {
        // Arrange
        var services = new ServiceCollection();
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var prefix = testAssemblyName!.Substring(0, 10);

        // Act
        services.AddScrutorAttributesForAssembliesStartingWith(prefix);
        services.AddScrutorAttributesForAssembliesStartingWith(prefix);

        // Assert - Should only have one registration due to Skip strategy
        var singletonServices = services.Where(s => s.ServiceType == typeof(ISingletonService)).ToList();
        Assert.That(singletonServices.Count, Is.EqualTo(1), "Should not register duplicate services");
    }

    [Test]
    public void AddScrutorAttributesForAssembliesContaining_CalledMultipleTimes_ShouldSkipDuplicates() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForAssembliesContaining("Scrutor");
        services.AddScrutorAttributesForAssembliesContaining("Scrutor");

        // Assert - Should only have one registration due to Skip strategy
        var singletonServices = services.Where(s => s.ServiceType == typeof(ISingletonService)).ToList();
        Assert.That(singletonServices.Count, Is.EqualTo(1), "Should not register duplicate services");
    }

    [Test]
    public void AddScrutorAttributesForAssembliesStartingWith_AllLifetimeInterfaces_ShouldBeRegistered() {
        // Arrange
        var services = new ServiceCollection();
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var prefix = testAssemblyName!.Substring(0, 10);

        // Act
        services.AddScrutorAttributesForAssembliesStartingWith(prefix);
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
    public void AddScrutorAttributesForAssembliesContaining_AllLifetimeInterfaces_ShouldBeRegistered() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForAssembliesContaining("Scrutor");
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
    public void AddScrutorAttributesForAssembliesStartingWith_EmptyPrefix_ShouldMatchAllAssemblies() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForAssembliesStartingWith("");
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Empty prefix matches everything
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null);
    }

    [Test]
    public void AddScrutorAttributesForAssembliesContaining_EmptyText_ShouldMatchAllAssemblies() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrutorAttributesForAssembliesContaining("");
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Empty text matches everything
        Assert.That(serviceProvider.GetService<ISingletonService>(), Is.Not.Null);
    }
}
