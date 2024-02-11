namespace MediaPauser.Platforms.Android.TimerService;

internal sealed record TimerStatus(bool IsRunning, DateTimeOffset? StartTime, TimeSpan? Duration);