using ScrutorAttributes;

namespace WebApiExample.Services;

/// <summary>
/// Provides greeting messages with scoped lifetime.
/// This service will be automatically registered as Scoped because it has attribute InjectedScoped.
/// </summary>
[InjectedScoped]

public class GreetingService : IGreetingService {
    /// <summary>
    /// Gets a personalized greeting message.
    /// </summary>
    /// <param name="name">The name to include in the greeting.</param>
    /// <returns>A personalized greeting message.</returns>
    public string GetGreeting(string name) {
        return $"Hello, {name}! This service was auto-registered using ScrutorAttributes.";
    }
}
