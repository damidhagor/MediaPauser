namespace MediaPauser.Extensions;

internal static class DateTimeExtensions
{
    public static DateTimeOffset TruncateMilliseconds(this DateTimeOffset dateTime)
    {
        return new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Offset);
    }

    public static DateTimeOffset TruncateMilliseconds(this DateTime dateTime)
    {
        return TruncateMilliseconds(new DateTimeOffset(dateTime));
    }
}
