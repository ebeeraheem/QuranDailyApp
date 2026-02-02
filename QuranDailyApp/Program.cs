using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuranDailyApp;
using QuranDailyApp.Core.Services;
using QuranDailyApp.Core.Interfaces;
using QuranDailyApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMemoryCache();

// Add file service for Blazor
builder.Services.AddScoped<IFileService, BlazorFileService>();

builder.Services.AddScoped<QuranService>();

await builder.Build().RunAsync();
