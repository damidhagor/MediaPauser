namespace MediaPauser.Platforms.Android.TimerService;

internal sealed class TimerStartedEventArgs(TimerStatus status) : EventArgs
{
    public TimerStatus TimerStatus { get; } = status;
}
