using CommunityToolkit.Maui;
using MediaPauser.ViewModels;
using Microsoft.Extensions.Logging;

namespace MediaPauser;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwesome-6-Regular-400.otf", "FontAwesome");
                fonts.AddFont("FontAwesome-6-Solid-900.otf", "FontAwesomeSolid");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<IMessenger, WeakReferenceMessenger>();

        builder.Services.AddSingleton(Preferences.Default);
        builder.Services.AddTransient<ISettingsService, SettingsService>();

        builder.Services.AddSingleton<TimerServiceConnection>();
        builder.Services.AddTransient<ITimerServiceAccessor, TimerServiceAccessor>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();

        return builder.Build();
    }
}
