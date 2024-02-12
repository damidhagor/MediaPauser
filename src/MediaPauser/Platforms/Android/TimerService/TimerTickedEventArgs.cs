namespace MediaPauser.Platforms.Android.TimerService;

internal sealed class TimerTickedEventArgs(DateTimeOffset startTime, TimeSpan duration, TimeSpan remainingTime) : EventArgs
{
    public DateTimeOffset StartTime { get; } = startTime;

    public TimeSpan Duration { get; } = duration;

    public TimeSpan RemainingTime { get; } = remainingTime;
}