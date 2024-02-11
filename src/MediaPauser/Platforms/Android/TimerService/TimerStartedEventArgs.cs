namespace MediaPauser.Platforms.Android.TimerService;

internal sealed class TimerStartedEventArgs(DateTimeOffset startTime, TimeSpan duration) : EventArgs
{
    public DateTimeOffset StartTime { get; } = startTime;

    public TimeSpan Duration { get; } = duration;
}
