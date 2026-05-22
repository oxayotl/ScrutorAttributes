using ScrutorAttributes;

namespace BlazorServerExample.Services;

/// <summary>
/// Implements counter functionality with singleton lifetime.
/// Auto-registered as Singleton using ScrutorAttributes.
/// </summary>
[InjectedSingleton]
public class CounterService : ICounterService {
    /// <summary>
    /// The internal counter value.
    /// </summary>
    private int _count = 0;

    /// <summary>
    /// Gets the current count value.
    /// </summary>
    /// <returns>The current count.</returns>
    public int GetCount() => _count;

    /// <summary>
    /// Increments the counter by one.
    /// </summary>
    public void Increment() => _count++;
}
