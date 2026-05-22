using ScrutorAttributes;

namespace WebApiExample.Services;

/// <summary>
/// Provides date and time services with singleton lifetime.
/// This service will be automatically registered as Singleton because it implements ISingletonLifetime.
/// </summary>
[InjectedSingleton]
public class DateTimeService : IDateTimeService {
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    /// <returns>The current UTC date and time.</returns>
    public DateTime GetCurrentDateTime() {
        return DateTime.UtcNow;
    }
}
