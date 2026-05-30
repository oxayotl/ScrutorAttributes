using ScrutorAttributes.Test.Assembly;
using ScrutorAttributes.Tests.TestServices;
namespace ScrutorAttributes.Tests;

[TestFixture]
public class RegisterAsTests {

    [Test]
    public void InjectionAs_WhenEverything() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes(RegisterAs.Everything);
        var serviceProvider = services.BuildServiceProvider();

        Assert.Multiple(() => {
            Assert.That(serviceProvider.GetService<InterfaceImplementation>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<ILibraryInterface>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<InternalLibraryInterfaceImplementation>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IInternalLibraryInterface>(), Is.Not.Null);
        });
    }
    [Test]
    public void InjectionAs_WhenSelf() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes(RegisterAs.Self);
        var serviceProvider = services.BuildServiceProvider();

        Assert.Multiple(() => {
            Assert.That(serviceProvider.GetService<InterfaceImplementation>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<ILibraryInterface>(), Is.Null);
            Assert.That(serviceProvider.GetService<InternalLibraryInterfaceImplementation>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IInternalLibraryInterface>(), Is.Null);
        });
    }

    [Test]
    public void InjectionAs_WhenInterfaces() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes(RegisterAs.Interfaces);
        var serviceProvider = services.BuildServiceProvider();

        Assert.Multiple(() => {
            Assert.That(serviceProvider.GetService<InterfaceImplementation>(), Is.Null);
            Assert.That(serviceProvider.GetService<ILibraryInterface>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<InternalLibraryInterfaceImplementation>(), Is.Null);
            Assert.That(serviceProvider.GetService<IInternalLibraryInterface>(), Is.Not.Null);
        });
    }

    [Test]
    public void InjectionAs_WhenOneInterfaceOrSelf() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes(RegisterAs.OneInterfaceOrSelf);
        var serviceProvider = services.BuildServiceProvider();

        Assert.Multiple(() => {
            Assert.That(serviceProvider.GetService<InterfaceImplementation>(), Is.Null);
            Assert.That(serviceProvider.GetService<ILibraryInterface>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<InternalLibraryInterfaceImplementation>(), Is.Null);
            Assert.That(serviceProvider.GetService<IInternalLibraryInterface>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<LibraryClass>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<DoubleInterfaceService>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IInterface1>(), Is.Null);
            Assert.That(serviceProvider.GetService<IInterface2>(), Is.Null);
        });
    }

    [Test]
    public void InjectionAs_WhenInterfacesFromSameAssembly() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes(RegisterAs.InterfacesFromSameAssembly);
        var serviceProvider = services.BuildServiceProvider();

        Assert.Multiple(() => {
            Assert.That(serviceProvider.GetService<InterfaceImplementation>(), Is.Null);
            Assert.That(serviceProvider.GetService<ILibraryInterface>(), Is.Null);
            Assert.That(serviceProvider.GetService<InternalLibraryInterfaceImplementation>(), Is.Null);
            Assert.That(serviceProvider.GetService<IInternalLibraryInterface>(), Is.Not.Null);
        });
    }

    [Test]
    public void InjectionAs_WhenSelfAndInterfacesFromSameAssembly() {
        var services = new ServiceCollection();
        services.AddScrutorAttributes(RegisterAs.SelfAndInterfacesFromSameAssembly);
        var serviceProvider = services.BuildServiceProvider();

        Assert.Multiple(() => {
            Assert.That(serviceProvider.GetService<InterfaceImplementation>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<ILibraryInterface>(), Is.Null);
            Assert.That(serviceProvider.GetService<InternalLibraryInterfaceImplementation>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IInternalLibraryInterface>(), Is.Not.Null);
        });
    }
}
