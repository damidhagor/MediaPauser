using Android.Content;
using MediaPauser.Platforms.Android.TimerService;

namespace MediaPauser.Platforms.Android.Services;

internal sealed class TimerServiceAccessor(TimerServiceConnection serviceConnection) : ITimerServiceAccessor
{
    private readonly Context _context = global::Android.App.Application.Context;
    private readonly TimerServiceConnection _serviceConnection = serviceConnection;

    public void StartTimer(TimeSpan duration)
    {
        var startIntent = new Intent(_context, typeof(TimerService.TimerService));
        startIntent.SetAction(Constants.StartTimerServiceAction);
        startIntent.PutExtra(Constants.TimerDurationExtraName, duration.Ticks);

        _context.StartForegroundService(startIntent);
    }

    public void StopTimer()
    {
        var stopIntent = new Intent(_context, typeof(TimerService.TimerService));
        stopIntent.SetAction(Constants.StopTimerServiceAction);

        _context.StartService(stopIntent);
    }

    public void IncrementTimer(TimeSpan increment)
    {
        _serviceConnection.Service?.IncrementTimer(increment);
    }

    public void BindTimerService()
    {
        var intent = new Intent(_context, typeof(TimerService.TimerService));
        _context.BindService(intent, _serviceConnection, Bind.AutoCreate);
    }

    public void UnbindTimerService()
    {
        _context.UnbindService(_serviceConnection);
    }
}
