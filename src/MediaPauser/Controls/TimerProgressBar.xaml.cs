namespace MediaPauser.Controls;

internal partial class TimerProgressBar
{
    public static readonly BindableProperty DurationProperty =
        BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(TimeSelector), TimeSpan.Zero, propertyChanged: OnValueChanged);

    public static readonly BindableProperty RemainingTimeProperty =
        BindableProperty.Create(nameof(RemainingTime), typeof(TimeSpan), typeof(TimeSelector), TimeSpan.Zero, propertyChanged: OnValueChanged);

    public static readonly BindableProperty StrokeProperty =
        BindableProperty.Create(nameof(Stroke), typeof(Color), typeof(TimerProgressBarDrawable), Colors.White, propertyChanged: OnValueChanged);

    public static readonly BindableProperty StrokeSizeProperty =
        BindableProperty.Create(nameof(StrokeSize), typeof(float), typeof(TimerProgressBarDrawable), 1.0f, propertyChanged: OnValueChanged);

    public static readonly BindableProperty StrokeLineCapProperty =
        BindableProperty.Create(nameof(StrokeLineCap), typeof(LineCap), typeof(TimerProgressBarDrawable), LineCap.Butt, propertyChanged: OnValueChanged);

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TimerProgressBar timerProgressBar)
        {
            timerProgressBar.Invalidate();
        }
    }
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

    public TimerProgressBar()
    {
        InitializeComponent();
    }
}
