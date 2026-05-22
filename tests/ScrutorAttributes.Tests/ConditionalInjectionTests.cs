using ScrutorAttributes.Tests.TestServices;
using static ScrutorAttributes.Tests.TestServices.TestEnvironementVariable;
namespace ScrutorAttributes.Tests;

[TestFixture]
public class ConditionalInjectionTests {
    [SetUp]
    public void Setup() {
        Environment.SetEnvironmentVariable(Name, null);
    }

    [Test]
    public void InjectionIf_WhenNotSet_ShouldNotBeIncluded() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfService>(), Is.Null);
    }

    [Test]
    public void InjectionIf_WhenSet_ShouldBeIncluded() {
        Environment.SetEnvironmentVariable(Name, SameValue);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfService>(), Is.Not.Null);
    }

    [Test]
    public void InjectionIfNot_WhenNotSet_ShouldBeIncluded() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfNotService>(), Is.Not.Null);
    }

    [Test]
    public void InjectionIfNot_WhenSet_ShouldNotBeIncluded() {
        Environment.SetEnvironmentVariable(Name, SameValue);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfNotService>(), Is.Null);
    }

    [Test]
    public void InjectionIfEqual_WhenNotSet_ShouldNotBeIncluded() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfEqualService>(), Is.Null);
    }

    [Test]
    public void InjectionIfEqual_WhenSetToOtherValue_ShouldNotBeIncluded() {
        Environment.SetEnvironmentVariable(Name, DifferentValue);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfEqualService>(), Is.Null);
    }

    [Test]
    public void InjectionIfEqual_WhenSetToEqualValue_ShouldBeIncluded() {
        Environment.SetEnvironmentVariable(Name, SameValue);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfEqualService>(), Is.Not.Null);
    }

    [Test]
    public void InjectionIfEqual_WhenSetToEqualValueWithDifferentCase_ShouldBeIncluded() {
        Environment.SetEnvironmentVariable(Name, SameValueWithDifferentCase);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfEqualService>(), Is.Not.Null);
    }

    [Test]
    public void InjectionIfNotEqual_WhenSetToOtherValue_ShouldBeIncluded() {
        Environment.SetEnvironmentVariable(Name, DifferentValue);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfNotEqualService>(), Is.Not.Null);
    }

    [Test]
    public void InjectionIfNotEqual_WhenSetToEqualValue_ShouldNotBeIncluded() {
        Environment.SetEnvironmentVariable(Name, SameValue);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfNotEqualService>(), Is.Null);
    }

    [Test]
    public void InjectionIfNotEqual_WhenSetToEqualValueWithDifferentCase_ShouldNotBeIncluded() {
        Environment.SetEnvironmentVariable(Name, SameValueWithDifferentCase);
        var services = new ServiceCollection();
        services.AddScrutorAttributes();
        var serviceProvider = services.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<IfNotEqualService>(), Is.Null);
    }

}
