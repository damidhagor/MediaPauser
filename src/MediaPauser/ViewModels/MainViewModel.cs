using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MediaPauser.ViewModels;

internal sealed partial class MainViewModel : ObservableRecipient,
    IRecipient<TimerStarted>,
    IRecipient<TimerStopped>,
    IRecipient<TimerTicked>
{
    private readonly ISettingsService _settingsService;
    private readonly ITimerServiceAccessor _timerServiceAccessor;

    [ObservableProperty]
    private TimeSpan _duration;

    [ObservableProperty]
    private bool _timerIsRunning;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RemainingHours))]
    [NotifyPropertyChangedFor(nameof(RemainingMinutes))]
    [NotifyPropertyChangedFor(nameof(RemainingSeconds))]
    private TimeSpan _remainingTime = TimeSpan.Zero;

    public int RemainingHours => (int)RemainingTime.TotalHours;

    public int RemainingMinutes => RemainingTime.Minutes;

    public int RemainingSeconds => RemainingTime.Seconds;

    public MainViewModel(IMessenger messenger, ISettingsService settingsService, ITimerServiceAccessor timerServiceAccessor)
        : base(messenger)
    {
        _settingsService = settingsService;
        _timerServiceAccessor = timerServiceAccessor;
        Duration = _settingsService.GetLastUsedTimerDuration();
    }

    [RelayCommand]
    private void Activate()
    {
        IsActive = true;
        _timerServiceAccessor.BindTimerService();
    }

    [RelayCommand]
    private void Deactivate()
    {
        _timerServiceAccessor.UnbindTimerService();
        IsActive = false;
    }

    [RelayCommand]
    private void StartTimer() => _timerServiceAccessor.StartTimer(Duration);

    [RelayCommand]
    private void StopTimer() => _timerServiceAccessor.StopTimer();

    public void Receive(TimerStarted message)
    {
        _settingsService.StoreLastUsedTimerDuration(message.Duration);
        Duration = message.Duration;
        RemainingTime = message.Duration;
        TimerIsRunning = true;
    }

    public void Receive(TimerStopped message)
    {
        TimerIsRunning = false;
        RemainingTime = TimeSpan.Zero;
    }

    public void Receive(TimerTicked message)
    {
        RemainingTime = message.RemainingTime;
    }
}
