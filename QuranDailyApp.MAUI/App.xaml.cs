using Microsoft.Extensions.DependencyInjection;

namespace QuranDailyApp.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        LoadTheme();
    }

    private void LoadTheme()
    {
        var theme = Preferences.Get("Theme", "System");
        
        switch (theme)
        {
            case "Light":
                UserAppTheme = AppTheme.Light;
                break;
            case "Dark":
                UserAppTheme = AppTheme.Dark;
                break;
            default:
                UserAppTheme = AppTheme.Unspecified;
                break;
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}