using System.Timers;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Views;
using static MediaPauser.Platforms.Android.TimerService.Constants;
using static MediaPauser.Resources.AppResources;

namespace MediaPauser.Platforms.Android.TimerService;

[Service]
internal sealed class TimerService : Service, ITimerService
{
    private readonly TimeProvider _timeProvider;
    private readonly TimerServiceBinder _binder;
    private readonly Timer _timer;
    private NotificationManager? _notificationManager;
    private AudioManager? _audioManager;
    private DateTimeOffset _timerStartTime;
    private TimeSpan _timerDuration;

    public event EventHandler<TimerStartedEventArgs> TimerStarted = null!;
    public event EventHandler<TimerTickedEventArgs> TimerTicked = null!;
    public event EventHandler TimerStopped = null!;

    public TimerService()
    {
        _timeProvider = IPlatformApplication.Current?.Services.GetRequiredService<TimeProvider>()
            ?? TimeProvider.System;
        _binder = new(this);
        _timer = new(TimeSpan.FromSeconds(1));
        _timer.Elapsed += OnTimerElapsed;
        _timerDuration = TimeSpan.Zero;
    }

    public override void OnCreate()
    {
        base.OnCreate();

        _notificationManager = GetSystemService(NotificationService) as NotificationManager;
        _audioManager = GetSystemService(AudioService) as AudioManager;
    }

    public override void OnDestroy()
    {
        _timer.Stop();
        _timer.Elapsed -= OnTimerElapsed;
        _timer.Dispose();
        _binder.Dispose();

        base.OnDestroy();
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        base.OnStartCommand(intent, flags, startId);

        if (intent?.Action == StartTimerServiceAction
            && intent.HasExtra(TimerDurationExtraName))
        {
            var duration = intent.GetLongExtra(TimerDurationExtraName, 0);
            StartTimer(TimeSpan.FromTicks(duration));
            return StartCommandResult.Sticky;
        }
        else if (intent?.Action == StopTimerServiceAction)
        {
            StopTimer();
            return StartCommandResult.Sticky;
        }

        StopSelf(startId);
        return StartCommandResult.Sticky;
    }

    public override IBinder? OnBind(Intent? intent) => _binder;

    public TimerStatus GetTimerStatus() => new(
            _timer.Enabled,
            _timer.Enabled ? _timerStartTime : null,
            _timer.Enabled ? _timerDuration : null);

    private void StartTimer(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            StopTimer();
            return;
        }

        _timerDuration = duration;
        _timerStartTime = _timeProvider.GetUtcNow().TruncateMilliseconds();

        StartForegroundService();

        _timer.Start();
        TimerStarted?.Invoke(this, new(_timerStartTime, _timerDuration));
    }

    private void StopTimer()
    {
        _timer.Stop();
        TimerStopped?.Invoke(this, new());
        StopForegroundService();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var signalTime = e.SignalTime.TruncateMilliseconds();
        var elapsedTime = signalTime - _timerStartTime;
        var remainingTime = _timerDuration - elapsedTime;

        if (_notificationManager is not null)
        {
            var notification = BuildNotification(remainingTime);
            _notificationManager.Notify(NotificationId, notification);
        }

        if (remainingTime > TimeSpan.Zero)
        {
            TimerTicked?.Invoke(this, new(_timerStartTime, _timerDuration, remainingTime));
        }
        else
        {
            StopTimer();
            PauseMediaPlayback();
        }
    }

    private void PauseMediaPlayback()
    {
        if (_audioManager is null)
        {
            return;
        }

        _audioManager.DispatchMediaKeyEvent(new KeyEvent(KeyEventActions.Down, Keycode.MediaPause));
        _audioManager.DispatchMediaKeyEvent(new KeyEvent(KeyEventActions.Up, Keycode.MediaPause));
        _audioManager.Dispose();
    }

    private void StartForegroundService()
    {
        CreateNotificationChannel();

        var notification = BuildNotification(_timerDuration);

        StartForeground(NotificationId, notification);
    }

    private void StopForegroundService()
    {
        StopForeground(StopForegroundFlags.Remove);
        StopSelf();
    }

    private Notification BuildNotification(TimeSpan remainingTime)
    {
        var showMainActivityIntent = BuildShowMainActivityIntent();
        var stopServiceAction = BuildStopServiceAction();

        var title = _timer.Enabled ? NotificationTitle : "";
        var text = _timer.Enabled
            ? $"{(int)remainingTime.TotalHours:00}:{remainingTime.Minutes:00}:{remainingTime.Seconds:00}"
            : "";

        return new Notification.Builder(this, NotificationChannelId)
             .SetOngoing(true)
             .SetSmallIcon(Resource.Mipmap.appicon_foreground)
             .SetContentTitle(title)
             .SetContentText(text)
             .AddAction(stopServiceAction)
             .SetContentIntent(showMainActivityIntent)
             .Build();
    }

    private PendingIntent? BuildShowMainActivityIntent()
    {
        var intent = new Intent(this, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.SingleTop);

        return PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Immutable);
    }

    private Notification.Action BuildStopServiceAction()
    {
        var intent = new Intent(this, typeof(TimerService));
        intent.SetAction(StopTimerServiceAction);

        var pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.Immutable);

        return new Notification.Action.Builder(null, NotificationStopAction, pendingIntent).Build();
    }

    private void CreateNotificationChannel()
        => _notificationManager?.CreateNotificationChannel(new(NotificationChannelId, NotificationChannelName, NotificationImportance.Low));
}
