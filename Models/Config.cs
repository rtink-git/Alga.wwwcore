namespace Alga.wwwcore.Models;

/// <summary>
/// Config Model for app settings.
/// </summary>
public class Config
{
    // Current Version, format: 202507130901
    internal string CurrentVersion { get; set; } = string.Empty;
    // The base URL of the application depending on the build mode.
    // Example: "https://localhost:1234" (for dev) or "https://example.com" (for prod).
    public string Url { get; set; } = string.Empty;

    // Indicates whether the app is running in debug mode.
    // true = development, false = production.
    public bool IsDebug { get; set; }

    // The full name of the application. Example: "My Awesome App".
    public string Name { get; set; } = string.Empty;

    // Short name or abbreviation of the app. Example: "AwesomeApp".
    public string NameShort { get; set; } = string.Empty;

    // Brief description of the app. Can be used in meta tags, tooltips, etc.
    public string Description { get; set; } = string.Empty;

    // Default cache duration (in seconds) for all UI Screens.
    // Use -1 to disable caching.
    public int CacheControlInSDefault { get; set; }

    // List of domains to preconnect for faster resource loading.
    // Example: { "api.example.com", "cdn.example.com" }
    public List<string>? PreconnectUrls { get; set; }

    // Google Fonts CSS URL. Example: "https://fonts.googleapis.com/css2?family=Roboto&display=swap"
    public string? GoogleFontsUrl { get; init; }

    // Google Analytics tracking ID. Example: "G-XXXXXXX".
    public string? GoogleAnalyticsCode { get; init; }

    // Yandex Metrika tracking ID. Example: "12345678".
    public string? YandexMetrikaCode { get; init; }

    // Twitter handle for metadata. Example: "@example".
    public string? TwitterSite { get; init; }

    // The color used for background while app loads (for PWAs).
    // Example: "#FFFFFF" for white or "#000000" for black.
    public string BackgroundColor { get; set; } = "#FFFFFF";

    // Primary theme color for browser UI (used in manifest and meta).
    public string ThemeColor { get; set; } = "#FFFFFF";

    // App language code. Example: "en", "ru", etc.
    public string Lang { get; set; }

    // URL to offline fallback page used when offline.
    // Example: "/offline"
    public string offlinePageUrl { get; set; } = "/offline";

    // List of static URLs to be pre-cached by service worker.
    // Example: ["/index.html", "/app.js", "/style.css"]
    public string[] cacheUrls { get; set; }
    public bool UseMessagePack { get; set; }
    public bool UseTelegram { get; set; }
}




    /// <summary>
    /// Config Model for app settings.
    /// </summary>
    /// <param name="IsDebug">Build Mode. Indicates whether the application is running in debug mode. When <c>true</c>, the application is in debug mode (typically used for development). When <c>false</c>, the application is in production mode.</param>
    /// <param name="Url">The base URL of the application, which can vary based on the build mode. For example, in debug mode, it could be a local development URL and in production, it would be the live application's URL. Example: (IsDebug) ? <c>https://localhost:1234</c> : <c>https://example.com</c></param>
    /// <param name="Name">The full name of the application. Example: "My Awesome App".</param>
    /// <param name="NameShort">The short name or abbreviation of the application. Example: "AwesomeApp".</param>
    /// <param name="Description">A brief description of what the application does. This can be displayed in meta tags, tooltips, or other UI elements. Example: "A great app for managing your tasks."</param>
    /// <param name="CacheControlInSDefault">The default cache management duration (in seconds) for all application UI Screens. The default value is -1, indicating no cache control by default.</param>
    /// <param name="PreconnectUrls">A list of URLs for preconnect, which helps in optimizing loading performance. These are typically URLs of APIs or services that the app communicates with, allowing the browser to establish connections early. Example: new List<string>() { "api.example.com", "api1.example.com", "api2.example.com" }</param>
    /// <param name="GoogleFontsUrl">URL for including Google Fonts in the application. This URL can be generated from the Google Fonts website. Example: "https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap"</param>
    /// <param name="GoogleAnalyticsCode">The Google Analytics tracking code for the application. Example: "G-9BBDVVTTXX". This code is used to integrate Google Analytics tracking for analytics purposes.</param>
    /// <param name="YandexMetrikaCode">The Yandex Metrika tracking code for the application. Example: "99991111". Used for tracking and analytics via Yandex Metrika.</param>
    /// <param name="TwitterSite">The Twitter account handle associated with the application. Example: "@ElonMusk". This can be used for social media links or integrations.</param>
    /// <param name="BackgroundColor">The background color used when the application loads. This is particularly important for Progressive Web Apps (PWA) to show a consistent color while the app is loading. Example: "#FFFFFF" for white or "#000000" for black.</param>
    /// <param name="ThemeColor">The theme color used for the application's interface and visual elements. This can influence the color of the browser's address bar and other UI elements, particularly in mobile web apps and PWAs. Example: "#FFFFFF" for white or "#000000" for black.</param>
    /// <param name="Lang"></param>
    // public class ConfigModel {
    //     internal string Url {get; set;} = string.Empty;
    //     public bool IsDebug {get; set;}
    //     public string Name {get; set;} = string.Empty;
    //     public string NameShort {get; set;} = string.Empty;
    //     public string Description {get; set;} = string.Empty;
    //     public int CacheControlInSDefault {get; set;}
    //     public List<string>? PreconnectUrls {get; set;}
    //     public string? GoogleFontsUrl {get; init;}
    //     public string? GoogleAnalyticsCode {get; init;}
    //     public string? YandexMetrikaCode {get; init;}
    //     public string? TwitterSite {get; init;}
    //     public string BackgroundColor {get; set;} = "#FFFFFF";
    //     public string ThemeColor {get; set;} = "#FFFFFF";
    //     public string Lang {get; set;}
    //     public string offlinePageUrl {get; set;} = "/offline";
    //     public string[] cacheUrls {get; set;}
    // };