namespace MediaPauser.Controls;

internal sealed class TimerProgressBarDrawable : BindableObject, IDrawable
{
    public static readonly BindableProperty DurationProperty =
        BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(TimerProgressBarDrawable), TimeSpan.Zero);

    public static readonly BindableProperty RemainingTimeProperty =
        BindableProperty.Create(nameof(RemainingTime), typeof(TimeSpan), typeof(TimerProgressBarDrawable), TimeSpan.Zero);

    public static readonly BindableProperty StrokeProperty =
        BindableProperty.Create(nameof(Stroke), typeof(Color), typeof(TimerProgressBarDrawable), Colors.White);

    public static readonly BindableProperty StrokeSizeProperty =
        BindableProperty.Create(nameof(StrokeSize), typeof(float), typeof(TimerProgressBarDrawable), 1.0f);

    public static readonly BindableProperty StrokeLineCapProperty =
        BindableProperty.Create(nameof(StrokeLineCap), typeof(LineCap), typeof(TimerProgressBarDrawable), LineCap.Butt);

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public TimeSpan RemainingTime
    {
        get => (TimeSpan)GetValue(RemainingTimeProperty);
        set => SetValue(RemainingTimeProperty, value);
    }

    public Color Stroke
    {
        get => (Color)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public float StrokeSize
    {
        get => (float)GetValue(StrokeSizeProperty);
        set => SetValue(StrokeSizeProperty, value);
    }

    public LineCap StrokeLineCap
    {
        get => (LineCap)GetValue(StrokeLineCapProperty);
        set => SetValue(StrokeLineCapProperty, value);
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Stroke;
        canvas.StrokeSize = StrokeSize;
        canvas.StrokeLineCap = StrokeLineCap;

        var (centerX, centerY) = dirtyRect.Center;

        var size = Math.Min(dirtyRect.Width, dirtyRect.Height);
        var radius = size / 2.0f;

        var (startAngle, endAngle) = CalculateAngles(Duration, RemainingTime);

        if (startAngle == endAngle)
        {
            canvas.DrawCircle(centerX, centerY, radius);
        }
        else
        {
            var positionX = centerX - radius;
            var positionY = centerY - radius;
            canvas.DrawArc(positionX, positionY, size, size, startAngle, endAngle, false, false);
        }
    }

    private static (float startAngle, float endAngle) CalculateAngles(TimeSpan duration, TimeSpan remainingTime)
    {
        var percentage = duration <= TimeSpan.Zero || remainingTime <= TimeSpan.Zero
            ? 1.0f
            : (float)remainingTime.Ticks / duration.Ticks;

        var startAngle = 90.0f;
        var endAngle = (startAngle + (360.0f * percentage)) % 360.0f;

        return (startAngle, endAngle);
    }
}