namespace Alga.wwwcore;

public class ClientOptions
{
    // The base URL of the application depending on the build mode.
    // Example: "https://localhost:1234" (for dev) or "https://example.com" (for prod).
    public required string BaseUrl { get; init; }

    // The full name of the application. Example: "My Awesome App".
    public required string Name { get; init; }

    // Short name or abbreviation of the app. Example: "AwesomeApp".
    public required string NameShort { get; init; }

    // Brief description of the app. Can be used in meta tags, tooltips, etc.
    public required string Description { get; init; }

    // The color used for background while app loads (for PWAs).
    // Example: "#FFFFFF" for white or "#000000" for black.
    public string BackgroundColor { get; set; } = "#FFFFFF";

    // Primary theme color for browser UI (used in manifest and meta).
    public string ThemeColor { get; set; } = "#FFFFFF";

    // List of domains to preconnect for faster resource loading.
    // Example: { "api.example.com", "cdn.example.com" }
    public string[]? PreconnectUrls { get; init; }

    // List of static URLs to be pre-cached by service worker.
    // Example: ["/index.html", "/app.js", "/style.css"]
    public string[]? CacheUrls { get; init; }

    public bool UseTelegram { get; set; }
    public bool UseMessagePack { get; set; }

    // https://www.bing.com/webmasters
    public string? BingSiteVerification { get; init; }

    // Twitter handle for metadata. Example: "@example".
    public string? TwitterSite { get; init; }

    // Webmaster yandex: https://webmaster.yandex.kz/
    public string? YandexVerificationCode { get; init; }

    // Yandex Metrika tracking ID. Example: "12345678".
    public string? YandexMetrikaCode { get; init; }

    // Google Analytics tracking ID. Example: "G-XXXXXXX".
    public string? GoogleAnalyticsCode { get; init; }

    // Google Fonts CSS URL. Example: "https://fonts.googleapis.com/css2?family=Roboto&display=swap"
    public string? GoogleFontsUrl { get; init; }

    // https://search.google.com/
    public string? GoogleSiteVerification { get; init; }

    // URL to offline fallback page used when offline.
    // Example: "/offline"
    public string? OfflinePageUrlPath { get; init; }
}