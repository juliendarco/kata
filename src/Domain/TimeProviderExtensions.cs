namespace LibraryManagement.Domain;

internal static class TimeProviderExtensions
{
    public static DateOnly GetToday(this TimeProvider timeProvider)
        => DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
}