namespace MediaPauser.Services;

internal sealed class SettingsService(IPreferences preferences) : ISettingsService
{
    private const string LastUsedTimerDurationKey = "LastUsedTimerDuration";

    private readonly IPreferences _preferences = preferences;

    public void StoreLastUsedTimerDuration(TimeSpan duration)
    {
        _preferences.Set(LastUsedTimerDurationKey, duration.Ticks);
    }

    public TimeSpan GetLastUsedTimerDuration()
    {
        var ticks = _preferences.Get(LastUsedTimerDurationKey, 0L);
        return TimeSpan.FromTicks(ticks);
    }
}
