namespace MediaPauser.Platforms.Android.TimerService;

internal interface ITimerService
{
    event EventHandler<TimerStartedEventArgs> TimerStarted;

    event EventHandler<TimerTickedEventArgs> TimerTicked;

    event EventHandler TimerStopped;

    TimerStatus GetTimerStatus();
}
