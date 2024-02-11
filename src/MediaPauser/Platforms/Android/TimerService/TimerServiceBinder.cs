using Android.OS;

namespace MediaPauser.Platforms.Android.TimerService;

internal sealed class TimerServiceBinder(ITimerService timerService) : Binder
{
    public ITimerService TimerService { get; } = timerService;
}
