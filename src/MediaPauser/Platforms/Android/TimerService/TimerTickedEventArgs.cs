namespace MediaPauser.Platforms.Android.TimerService;

internal sealed class TimerTickedEventArgs(TimerStatus timerStatus) : EventArgs
{
    public TimerStatus TimerStatus { get; } = timerStatus;
}