using ScrutorAttributes;

namespace MvcExample.Services;

/// <summary>
/// Provides message generation with transient lifetime.
/// Auto-registered as Transient using ScrutorAttributes.
/// </summary>
[InjectedTransient]
public class MessageService : IMessageService {
    /// <summary>
    /// Gets a message string about the service registration.
    /// </summary>
    /// <returns>A message string.</returns>
    public string GetMessage() {
        return "This service was automatically registered using ScrutorAttributes with Transient lifetime!";
    }
}
