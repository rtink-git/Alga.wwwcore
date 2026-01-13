namespace Alga.wwwcore.Operations.GetClientOptions;

public record Res(
    // The base URL of the application depending on the build mode.
    // Example: "https://localhost:1234" (for dev) or "https://example.com" (for prod).
    string BaseUrl,
    // The full name of the application. Example: "My Awesome App".
    string Name,
    // Short name or abbreviation of the app. Example: "AwesomeApp".
    string NameShort,
    // Brief description of the app. Can be used in meta tags, tooltips, etc.
    string Description,
    // The color used for background while app loads (for PWAs).
    // Example: "#FFFFFF" for white or "#000000" for black.
    string BackgroundColor,
    // Primary theme color for browser UI (used in manifest and meta).
    string ThemeColor,
    // List of domains to preconnect for faster resource loading.
    // Example: { "api.example.com", "cdn.example.com" }
    string[]? PreconnectUrls,
    // List of static URLs to be pre-cached by service worker.
    // Example: ["/index.html", "/app.js", "/style.css"]
    string[]? CacheUrls,
    bool UseTelegram,
    bool UseMessagePack,
    // https://www.bing.com/webmasters
    string? BingSiteVerification,
    // Twitter handle for metadata. Example: "@example".
    string? TwitterSite,
    // Webmaster yandex: https://webmaster.yandex.kz/
    string? YandexVerificationCode,
    // Yandex Metrika tracking ID. Example: "12345678".
    string? YandexMetrikaCode,
    // Google Analytics tracking ID. Example: "G-XXXXXXX".
    string? GoogleAnalyticsCode,
    // Google Fonts CSS URL. Example: "https://fonts.googleapis.com/css2?family=Roboto&display=swap"
    string? GoogleFontsUrl,
    // https://search.google.com/
    string? GoogleSiteVerification,
    // URL to offline fallback page used when offline.
    // Example: "/offline"
    string? OfflinePageUrlPath
);
