namespace MediaPauser.Services;

internal interface ISettingsService
{
    void StoreLastUsedTimerDuration(TimeSpan duration);

    TimeSpan GetLastUsedTimerDuration();
}
