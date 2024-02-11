namespace MediaPauser.Messages;

internal sealed record TimerStarted(DateTimeOffset StartTime, TimeSpan Duration);
