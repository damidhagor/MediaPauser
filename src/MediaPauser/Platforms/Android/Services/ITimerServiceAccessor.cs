namespace MediaPauser.Platforms.Android.Services;

internal interface ITimerServiceAccessor
{
    void StartTimer(TimeSpan duration);

    void StopTimer();

    void IncrementTimer(TimeSpan increment);

    void BindTimerService();

    void UnbindTimerService();
}
