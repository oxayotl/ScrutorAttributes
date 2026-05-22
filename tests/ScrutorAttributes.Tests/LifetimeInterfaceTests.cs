using ScrutorAttributes;
using System.Reflection;

namespace ScrutorAttribute.Tests;

[TestFixture]
public class LifetimeInterfaceTests {
    [Test]
    public void InjectedSingleton_AttributeShouldExist() {
        // Arrange & Act
        var type = typeof(InjectedSingleton);

        // Assert
        Assert.That(type, Is.Not.Null);
        Assert.That(type.IsAssignableTo(typeof(Attribute)), Is.True);
        Assert.That(type.Namespace, Is.EqualTo("ScrutorAttributes"));
    }

    [Test]
    public void InjectedTransient_AttributeShouldExist() {
        // Arrange & Act
        var type = typeof(InjectedTransient);

        // Assert
        Assert.That(type, Is.Not.Null);
        Assert.That(type.IsAssignableTo(typeof(Attribute)), Is.True);
        Assert.That(type.Namespace, Is.EqualTo("ScrutorAttributes"));
    }

    [Test]
    public void InjectedScoped_AttributeShouldExist() {
        // Arrange & Act
        var type = typeof(InjectedScoped);

        // Assert
        Assert.That(type, Is.Not.Null);
        Assert.That(type.IsAssignableTo(typeof(Attribute)), Is.True);
        Assert.That(type.Namespace, Is.EqualTo("ScrutorAttributes"));
    }

    //[Test]
    //public void AllLifetimeInterfaces_ShouldHaveNoMembers() {
    //    // Arrange
    //    var lifetimeTypes = new[]
    //    {
    //        typeof(ISingletonLifetime),
    //        typeof(ISelfSingletonLifetime),
    //        typeof(ITransientLifetime),
    //        typeof(ISelfTransientLifetime),
    //        typeof(IScopedLifetime),
    //        typeof(ISelfScopedLifetime)
    //    };

    //    // Act & Assert
    //    foreach (var type in lifetimeTypes) {
    //        var members = type.GetMembers();
    //        // Only inherited members from object should be present
    //        Assert.That(members.Length, Is.EqualTo(0),
    //            $"{type.Name} should be a marker interface with no members");
    //    }
    //}

    [Test]
    public void LifetimeAttributes_ShouldNotInheritFromEachOther() {
        // Arrange
        var lifetimeTypes = new[]
        {
            typeof(InjectedScoped),
            typeof(InjectedSingleton),
            typeof(InjectedTransient),
        };

        // Act & Assert
        foreach (var type1 in lifetimeTypes) {
            foreach (var type2 in lifetimeTypes.Where(type2 => type2 != type1)) {
                Assert.That(type1.IsAssignableFrom(type2), Is.False,
                    $"{type2.Name} should not inherit from {type1.Name}");
            }
        }
    }
}
