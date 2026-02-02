namespace QuranDailyApp.MAUI;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadCurrentTheme();
    }

    private void LoadCurrentTheme()
    {
        var theme = Preferences.Get("Theme", "System");

        switch (theme)
        {
            case "Light":
                LightThemeRadio.IsChecked = true;
                break;
            case "Dark":
                DarkThemeRadio.IsChecked = true;
                break;
            default:
                SystemThemeRadio.IsChecked = true;
                break;
        }
    }

    private void OnThemeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;

        var radioButton = sender as RadioButton;
        string theme = "";

        if (radioButton == LightThemeRadio)
        {
            theme = "Light";
            Application.Current!.UserAppTheme = AppTheme.Light;
        }
        else if (radioButton == DarkThemeRadio)
        {
            theme = "Dark";
            Application.Current!.UserAppTheme = AppTheme.Dark;
        }
        else if (radioButton == SystemThemeRadio)
        {
            theme = "System";
            Application.Current!.UserAppTheme = AppTheme.Unspecified;
        }

        Preferences.Set("Theme", theme);
    }
}