namespace MediaPauser.Platforms.Android.Services;

internal interface ITimerServiceAccessor
{
    void StartTimer(TimeSpan duration);

    void StopTimer();

    void BindTimerService();

    void UnbindTimerService();
}
