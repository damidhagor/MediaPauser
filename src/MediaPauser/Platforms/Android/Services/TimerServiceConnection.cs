using System.Timers;
using Android.Content;
using Android.OS;
using MediaPauser.Platforms.Android.TimerService;

namespace MediaPauser.Platforms.Android.Services;

internal sealed class TimerServiceConnection : Java.Lang.Object, IServiceConnection
{
    private readonly IMessenger _messenger;
    private readonly TimeProvider _timeProvider;
    private readonly Timer _statusTimer;
    private TimerServiceBinder? _binder;

    public bool IsConnected => Service is not null;

    public ITimerService? Service => _binder?.TimerService;

    public TimerServiceConnection(IMessenger messenger, TimeProvider timeProvider)
    {
        _messenger = messenger;
        _timeProvider = timeProvider;

        _statusTimer = new(TimeSpan.FromSeconds(1));
        _statusTimer.Elapsed += OnStatusTimerElapsed;
    }

    public void OnServiceConnected(ComponentName? name, IBinder? service)
    {
        _binder = service as TimerServiceBinder;

        if (Service is not null)
        {
            Service.TimerStarted -= OnTimerStarted;
            Service.TimerStarted += OnTimerStarted;
            Service.TimerStopped -= OnTimerStopped;
            Service.TimerStopped += OnTimerStopped;

            var status = Service.GetTimerStatus();
            if (status.IsRunning)
            {
                StartStatusTimer();
                BroadcastTimerStatus();
            }
        }
    }

    public void OnServiceDisconnected(ComponentName? name)
    {
        StopStatusTimer();
        _binder = null;
    }

    protected override void Dispose(bool disposing)
    {
        _statusTimer.Elapsed -= OnStatusTimerElapsed;
        _statusTimer.Stop();
        _statusTimer.Dispose();

        if (Service is not null)
        {
            Service.TimerStarted -= OnTimerStarted;
            Service.TimerStopped -= OnTimerStopped;
        }

        base.Dispose(disposing);
    }

    private void OnTimerStarted(object? sender, EventArgs e) => StartStatusTimer();

    private void OnTimerStopped(object? sender, EventArgs e) => StopStatusTimer();

    private void OnStatusTimerElapsed(object? sender, ElapsedEventArgs e) => BroadcastTimerStatus();

    private void StartStatusTimer()
    {
        _statusTimer.Start();
        var status = _binder?.TimerService?.GetTimerStatus();

        if (status is not null
            && status.StartTime is not null
            && status.Duration is not null)
        {
            _messenger.Send(new TimerStarted(status.StartTime.Value, status.Duration.Value));
        }
    }

    private void StopStatusTimer()
    {
        _statusTimer.Stop();
        _messenger.Send<TimerStopped>();
    }

    private void BroadcastTimerStatus()
    {
        var status = _binder?.TimerService?.GetTimerStatus();

        if (status is not null
            && status.StartTime is not null
            && status.Duration is not null)
        {
            var started = status.StartTime.Value;
            var duration = status.Duration.Value;
            var elapsed = _timeProvider.GetUtcNow() - started;
            var remaining = duration - elapsed;

            _messenger.Send(new TimerTicked(started, duration, remaining));
        }
    }
}