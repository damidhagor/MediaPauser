namespace MediaPauser.Messages;

internal sealed record TimerTicked(DateTimeOffset StartTime, TimeSpan Duration, TimeSpan RemainingTime);