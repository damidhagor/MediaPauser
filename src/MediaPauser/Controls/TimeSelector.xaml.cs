using static MediaPauser.Resources.AppResources;

namespace MediaPauser.Controls;

internal partial class TimeSelector
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(TimeSpan), typeof(TimeSelector), TimeSpan.Zero, BindingMode.TwoWay, propertyChanged: OnValueChanged);

    public static readonly BindableProperty HoursStepProperty =
        BindableProperty.Create(nameof(HoursStep), typeof(TimeSpan), typeof(TimeSelector), TimeSpan.FromHours(1));

    public static readonly BindableProperty MinutesStepProperty =
        BindableProperty.Create(nameof(MinutesStep), typeof(TimeSpan), typeof(TimeSelector), TimeSpan.FromMinutes(5));

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TimeSelector timeSelector)
        {
            timeSelector.OnPropertyChanged(nameof(Hours));
            timeSelector.OnPropertyChanged(nameof(Minutes));
        }
    }

    public TimeSpan Value
    {
        get => (TimeSpan)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public TimeSpan HoursStep
    {
        get => (TimeSpan)GetValue(HoursStepProperty);
        set => SetValue(HoursStepProperty, value);
    }

    public TimeSpan MinutesStep
    {
        get => (TimeSpan)GetValue(MinutesStepProperty);
        set => SetValue(MinutesStepProperty, value);
    }

    public int Hours => (int)Value.TotalHours;

    public int Minutes => Value.Minutes;

    public TimeSelector()
    {
        InitializeComponent();
    }

    private void IncreaseHours_Clicked(object sender, EventArgs e) => Value += HoursStep;

    private void DecreaseHours_Clicked(object sender, EventArgs e)
    {
        var newValue = Value - HoursStep;
        Value = newValue > TimeSpan.Zero ? newValue : TimeSpan.Zero;
    }

    private void IncreaseMinutes_Clicked(object sender, EventArgs e)
    {
        Value += TimeSpan.FromMinutes(5 - (Value.Minutes % 5));
    }

    private void DecreaseMinutes_Clicked(object sender, EventArgs e)
    {
        var minutesToSubtract = Value.Minutes % 5 == 0 ? 5 : Value.Minutes % 5;
        var newValue = Value - TimeSpan.FromMinutes(minutesToSubtract);
        Value = newValue > TimeSpan.Zero ? newValue : TimeSpan.Zero;
    }

    private async void Hours_Tapped(object sender, TappedEventArgs e)
    {
        var mainPage = Application.Current?.MainPage;
        if (mainPage is null)
        {
            return;
        }

        var input = await mainPage.DisplayPromptAsync("", "", Ok, Cancel, EnterHoursMessage, 2, Keyboard.Numeric);
        if (int.TryParse(input, out var hours))
        {
            Value = new(hours, Value.Minutes, 0);
        }
    }

    private async void Minutes_Tapped(object sender, TappedEventArgs e)
    {
        var mainPage = Application.Current?.MainPage;
        if (mainPage is null)
        {
            return;
        }

        var input = await mainPage.DisplayPromptAsync("", "", Ok, Cancel, EnterMinutesMessage, 2, Keyboard.Numeric);
        if (int.TryParse(input, out var minutes))
        {
            Value = new((int)Value.TotalHours, minutes, 0);
        }
    }
}