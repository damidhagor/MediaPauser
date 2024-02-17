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

    private TimerStatus _timerStatus = new(false, DateTimeOffset.MinValue, TimeSpan.Zero, TimeSpan.Zero);

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
        else if (intent?.Action == IncrementTimerAction
            && intent.HasExtra(TimerIncrementExtraName))
        {
            var increment = intent.GetLongExtra(TimerIncrementExtraName, 0);
            IncrementTimer(TimeSpan.FromTicks(increment));
            return StartCommandResult.Sticky;
        }

        StopSelf(startId);
        return StartCommandResult.Sticky;
    }

    public override IBinder? OnBind(Intent? intent) => _binder;

    public TimerStatus GetTimerStatus() => _timerStatus;

    private void StartTimer(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            StopTimer();
            return;
        }

        _timerStatus = new(true, _timeProvider.GetUtcNow().TruncateMilliseconds(), duration, duration);

        StartForegroundService();

        _timer.Start();
        TimerStarted?.Invoke(this, new(_timerStatus));
    }

    private void StopTimer()
    {
        _timer.Stop();
        _timerStatus = new(false, DateTimeOffset.MinValue, TimeSpan.Zero, TimeSpan.Zero);
        TimerStopped?.Invoke(this, new());
        StopForegroundService();
    }

    public void IncrementTimer(TimeSpan increment)
    {
        var newDuration = _timerStatus.Duration + increment;
        var remainingTime = CalculateRemainingTime(_timerStatus.StartTime, _timeProvider.GetUtcNow(), newDuration);
        _timerStatus = _timerStatus with { Duration = newDuration, RemainingTime = remainingTime };
        TimerTicked?.Invoke(this, new(_timerStatus));
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var remainingTime = CalculateRemainingTime(_timerStatus.StartTime, e.SignalTime, _timerStatus.Duration);
        _timerStatus = _timerStatus with { RemainingTime = remainingTime };

        if (_notificationManager is not null)
        {
            var notification = BuildNotification();
            _notificationManager.Notify(NotificationId, notification);
        }

        if (remainingTime > TimeSpan.Zero)
        {
            TimerTicked?.Invoke(this, new(_timerStatus));
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

        var notification = BuildNotification();

        StartForeground(NotificationId, notification);
    }

    private void StopForegroundService()
    {
        StopForeground(StopForegroundFlags.Remove);
        StopSelf();
    }

    private Notification BuildNotification()
    {
        var showMainActivityIntent = BuildShowMainActivityIntent();
        var stopServiceAction = BuildStopServiceAction();
        var incrementTimerAction = BuildIncrementTimerAction();

        var text = $"{(int)_timerStatus.RemainingTime.TotalHours:00}:{_timerStatus.RemainingTime.Minutes:00}:{_timerStatus.RemainingTime.Seconds:00}";

        return new Notification.Builder(this, NotificationChannelId)
             .SetOngoing(true)
             .SetOnlyAlertOnce(true)
             .SetSmallIcon(Resource.Mipmap.appicon_foreground)
             .SetContentTitle(NotificationTitle)
             .SetContentText(text)
             .AddAction(stopServiceAction)
             .AddAction(incrementTimerAction)
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

    private Notification.Action BuildIncrementTimerAction()
    {
        var intent = new Intent(this, typeof(TimerService));
        intent.SetAction(IncrementTimerAction);
        intent.PutExtra(TimerIncrementExtraName, TimeSpan.FromMinutes(5).Ticks);

        var pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.Immutable);

        return new Notification.Action.Builder(null, NotificationIncrementTimerAction, pendingIntent).Build();
    }

    private void CreateNotificationChannel()
        => _notificationManager?.CreateNotificationChannel(new(NotificationChannelId, NotificationChannelName, NotificationImportance.Low));

    private static TimeSpan CalculateRemainingTime(DateTimeOffset startTime, DateTimeOffset currentTime, TimeSpan duration)
    {
        var truncatedStartTime = startTime.TruncateMilliseconds();
        var truncatedCurrentTime = currentTime.TruncateMilliseconds();
        var elapsedTime = truncatedCurrentTime - truncatedStartTime;
        return duration - elapsedTime;
    }
}
