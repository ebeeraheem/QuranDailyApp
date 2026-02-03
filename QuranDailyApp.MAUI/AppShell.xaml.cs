namespace QuranDailyApp.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    private static async void OnEbeeSolutionsLinkTapped(object sender, TappedEventArgs e)
    {
        var uri = new Uri("https://ebeesolutions.com");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }
}
