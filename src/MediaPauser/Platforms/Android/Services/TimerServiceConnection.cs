using Android.Content;
using Android.OS;
using MediaPauser.Platforms.Android.TimerService;

namespace MediaPauser.Platforms.Android.Services;

internal sealed class TimerServiceConnection(IMessenger messenger)
    : Java.Lang.Object, IServiceConnection
{
    private readonly IMessenger _messenger = messenger;
    private TimerServiceBinder? _binder;

    public ITimerService? Service => _binder?.TimerService;

    public void OnServiceConnected(ComponentName? name, IBinder? service)
    {
        _binder = service as TimerServiceBinder;

        if (Service is not null)
        {
            Service.TimerStarted -= OnTimerStarted;
            Service.TimerStarted += OnTimerStarted;
            Service.TimerStopped -= OnTimerStopped;
            Service.TimerStopped += OnTimerStopped;
            Service.TimerTicked -= OnTimerTicked;
            Service.TimerTicked += OnTimerTicked;

            var status = Service.GetTimerStatus();
            if (status.IsRunning)
            {
                BroadcastTimerStarted(status.StartTime, status.Duration);
            }
        }
    }

    public void OnServiceDisconnected(ComponentName? name) => _binder = null;

    protected override void Dispose(bool disposing)
    {
        if (Service is not null)
        {
            Service.TimerStarted -= OnTimerStarted;
            Service.TimerStopped -= OnTimerStopped;
            Service.TimerTicked -= OnTimerTicked;
        }

        base.Dispose(disposing);
    }

    private void OnTimerStarted(object? sender, TimerStartedEventArgs e) => BroadcastTimerStarted(e.TimerStatus.StartTime, e.TimerStatus.Duration);

    private void OnTimerTicked(object? sender, TimerTickedEventArgs e) => BroadcastTimerTicked(e.TimerStatus.StartTime, e.TimerStatus.Duration, e.TimerStatus.RemainingTime);

    private void OnTimerStopped(object? sender, EventArgs e) => BroadcastTimerStopped();

    private void BroadcastTimerStarted(DateTimeOffset startTime, TimeSpan duration)
    {
        _messenger.Send(new TimerStarted(startTime, duration));
    }

    private void BroadcastTimerStopped()
    {
        _messenger.Send<TimerStopped>();
    }

    private void BroadcastTimerTicked(DateTimeOffset startTime, TimeSpan duration, TimeSpan remaining)
    {
        _messenger.Send(new TimerTicked(startTime, duration, remaining));
    }
}