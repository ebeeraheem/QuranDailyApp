# Porting Blazor PWA to MAUI Blazor Hybrid App

## Overview
This guide covers how to port your QuranDailyApp from Blazor WebAssembly PWA to a .NET MAUI Blazor Hybrid application while sharing the Core project.

## File Structure and Mapping

### 1. Static Files (CSS, JS, JSON Data)

**Blazor PWA Location:**
```
QuranDailyApp/wwwroot/
??? css/
?   ??? app.css
?   ??? bootstrap/bootstrap.min.css
??? js/
?   ??? app.js
??? data/
    ??? quran.json
```

**MAUI Location:**
```
QuranDailyApp.MAUI/wwwroot/
??? css/
?   ??? app.css
?   ??? bootstrap/bootstrap.min.css
??? js/
?   ??? app.js
??? data/
    ??? quran.json
```

**Action:** Copy the entire `wwwroot` folder from your Blazor project to the MAUI project, but exclude PWA-specific files (see below).

### 2. Razor Components

**Blazor PWA Location:**
```
QuranDailyApp/
??? App.razor
??? Layout/
?   ??? MainLayout.razor
??? Pages/
    ??? Home.razor
```

**MAUI Location:**
```
QuranDailyApp.MAUI/
??? Components/
?   ??? App.razor
?   ??? Layout/
?   ?   ??? MainLayout.razor
?   ??? Pages/
?       ??? Home.razor
```

**Action:** Copy your Razor components to the `Components` folder in the MAUI project.

### 3. App Icons

**PWA Icons (Not needed in MAUI):**
- `icon-192.png`
- `icon-512.png`

**MAUI Icons Location:**
```
QuranDailyApp.MAUI/Resources/Images/
??? appicon.svg  (or .png files for different sizes)
```

**Action:** Convert your PWA icons to MAUI app icons and place them in `Resources/Images/`.

### 4. Files NOT Needed in MAUI

These PWA-specific files are not needed in MAUI:
- ? `service-worker.js` - Not applicable in native apps
- ? `manifest.webmanifest` - Not applicable in native apps  
- ? `index.html` - Replaced by MainPage.xaml with BlazorWebView

## Key Differences Between PWA and MAUI

### 1. Application Entry Point

**PWA:** Uses `index.html` with Blazor WebAssembly
```html
<body>
    <div id="app">...</div>
    <script src="_framework/blazor.webassembly.js"></script>
</body>
```

**MAUI:** Uses `MainPage.xaml` with BlazorWebView
```xml
<ContentPage x:Class="QuranDailyApp.MAUI.MainPage">
    <BlazorWebView HostPage="wwwroot/index.html">
        <BlazorWebView.RootComponents>
            <RootComponent Selector="#app" ComponentType="{x:Type local:Components.App}" />
        </BlazorWebView.RootComponents>
    </BlazorWebView>
</ContentPage>
```

### 2. Service Registration

**PWA (Program.cs):**
```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
```

**MAUI (MauiProgram.cs):**
```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

    builder.Services.AddMauiBlazorWebView();
    // Add your services here
    builder.Services.AddScoped<QuranService>();
    
    return builder.Build();
}
```

### 3. Routing and Navigation

**PWA:** Uses Blazor Router
```razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
</Router>
```

**MAUI:** Same Router component, but hosted in BlazorWebView

### 4. HTTP Client Configuration

**PWA:** Uses browser's fetch API
```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
```

**MAUI:** Needs explicit HttpClient configuration
```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost/") });
// Or better yet, configure for local file access
builder.Services.AddScoped<HttpClient>();
```

## Step-by-Step Migration Process

### Step 1: Set up MAUI Project Structure
1. Create the following folders in your MAUI project:
   ```
   QuranDailyApp.MAUI/
   ??? Components/
   ?   ??? Layout/
   ?   ??? Pages/
   ??? wwwroot/
   ```

### Step 2: Copy and Adapt Files
1. **Copy static assets** from PWA `wwwroot` to MAUI `wwwroot` (excluding PWA files)
2. **Copy Razor components** to MAUI `Components` folder
3. **Convert app icons** to MAUI format

### Step 3: Update Service Registration
Update `MauiProgram.cs` to register your shared services:
```csharp
builder.Services.AddScoped<QuranService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
```

### Step 4: Update MainPage.xaml
Ensure your `MainPage.xaml` has the correct BlazorWebView setup:
```xml
<BlazorWebView HostPage="wwwroot/index.html">
    <BlazorWebView.RootComponents>
        <RootComponent Selector="#app" ComponentType="{x:Type components:App}" />
    </BlazorWebView.RootComponents>
</BlazorWebView>
```

### Step 5: Create Simple Host HTML
Create a minimal `wwwroot/index.html` for the MAUI app:
```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Quran Daily</title>
    <link href="css/app.css" rel="stylesheet" />
</head>
<body>
    <div id="app">Loading...</div>
    <script src="_framework/blazor.webview.js" autostart="false"></script>
</body>
</html>
```

## Platform-Specific Considerations

### Data Access
- **PWA:** Loads `quran.json` via HTTP from wwwroot
- **MAUI:** Can load from wwwroot or embed as resource

### Caching
- **PWA:** Uses service worker + browser cache
- **MAUI:** Uses MemoryCache (already implemented in your QuranService)

### Offline Support
- **PWA:** Service worker provides offline support
- **MAUI:** Naturally offline-first, data embedded in app

## Benefits of MAUI vs PWA

### MAUI Advantages:
? Native app distribution (App Store, Play Store)  
? Better performance (no browser overhead)  
? Native device integrations  
? Offline-first by design  
? Platform-specific UI adaptations  

### PWA Advantages:
? Single deployment for all platforms  
? Automatic updates  
? No app store approval process  
? Web-standard technologies  

## Shared Architecture

Since you're using the Core project, your architecture remains the same:
```
QuranDailyApp.Core (Shared)
??? Models/
?   ??? Ayah.cs
??? Services/
    ??? QuranService.cs

QuranDailyApp (PWA)          QuranDailyApp.MAUI (Native)
??? Pages/                   ??? Components/Pages/
??? Layout/            ?     ??? Components/Layout/  
??? wwwroot/                 ??? wwwroot/
??? App.razor                ??? Components/App.razor
```

Both apps can share:
- ? Business logic (QuranService)
- ? Data models (Ayah, QuranData)
- ? Razor components (with minor adaptations)
- ? CSS styles
- ? JavaScript functions

This allows you to maintain both PWA and native versions with minimal code duplication!