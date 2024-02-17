namespace MediaPauser.Platforms.Android.TimerService;

internal static class Constants
{
    public const string StartTimerServiceAction = "StartService";

    public const string StopTimerServiceAction = "StopService";

    public const string IncrementTimerAction = "IncrementTimer";

    public const string TimerDurationExtraName = "TimerDuration";

    public const string TimerIncrementExtraName = "TimerIncrement";

    public const int NotificationId = 1;

    public const string NotificationChannelId = "TimerNotificationChannel";

    public const string NotificationChannelName = "TimerNotificationChannel";
}
