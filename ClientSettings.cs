namespace Alga.wwwcore;

public sealed class ClientSettings
{
    // The base URL of the application depending on the build mode.
    // Example: "https://localhost:1234" (for dev) or "https://example.com" (for prod).
    public string BaseUrl { get; set; } = null!; // required

    // The full name of the application. Example: "My Awesome App".
    public string Name { get; set; } = null!; // required

    // Short name or abbreviation of the app. Example: "AwesomeApp".
    public string NameShort { get; set; } = null!; // required

    // Brief description of the app. Can be used in meta tags, tooltips, etc.
    public string Description { get; set; } = null!; // required

    // The color used for background while app loads (for PWAs).
    // Example: "#FFFFFF" for white or "#000000" for black.
    public string BackgroundColor { get; set; } = "#FFFFFF";

    // Primary theme color for browser UI (used in manifest and meta).
    public string ThemeColor { get; set; } = "#FFFFFF";

    // List of domains to preconnect for faster resource loading.
    // Example: { "api.example.com", "cdn.example.com" }
    public string[]? PreconnectUrls { get; set; }

    // List of static URLs to be pre-cached by service worker.
    // Example: ["/index.html", "/app.js", "/style.css"]
    public string[]? CacheUrls { get; set; }

    public bool UseTelegram { get; set; }

    public bool UseMessagePack { get; set; }

    // https://www.bing.com/webmasters
    public string? BingSiteVerification { get; set; }

    // Twitter handle for metadata. Example: "@example".
    public string? TwitterSite { get; set; }

    // Webmaster yandex: https://webmaster.yandex.kz/
    public string? YandexVerificationCode { get; set; }

    // Yandex Metrika tracking ID. Example: "12345678".
    public string? YandexMetrikaCode { get; set; }

    // Google Analytics tracking ID. Example: "G-XXXXXXX".
    public string? GoogleAnalyticsCode { get; set; }

    // Google Fonts CSS URL. Example: "https://fonts.googleapis.com/css2?family=Roboto&display=swap"
    public string? GoogleFontsUrl { get; set; }

    // https://search.google.com/
    public string? GoogleSiteVerification { get; set; }

    // URL to offline fallback page used when offline.
    // Example: "/offline"
    public string? OfflinePageUrlPath { get; set; }

    // public void Validate()
    // {
    //     if (string.IsNullOrWhiteSpace(BaseUrl))
    //         throw new InvalidOperationException($"{nameof(BaseUrl)} is missing or empty.");

    //     if (string.IsNullOrWhiteSpace(Name))
    //         throw new InvalidOperationException($"{nameof(Name)} is missing or empty.");

    //     if (string.IsNullOrWhiteSpace(NameShort))
    //         throw new InvalidOperationException($"{nameof(NameShort)} is missing or empty.");

    //     if (string.IsNullOrWhiteSpace(Description))
    //         throw new InvalidOperationException($"{nameof(Description)} is missing or empty.");
    // }
}