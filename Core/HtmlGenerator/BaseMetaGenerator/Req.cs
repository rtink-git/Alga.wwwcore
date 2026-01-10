namespace Alga.wwwcore.Core.HtmlGenerator.BaseMetaGenerator;

public class Req
{
    public required bool IsDebug { get; init; }
    public required string Version { get; init; }

    // Primary theme color for browser UI (used in manifest and meta).
    public string? ThemeColor { get; init; }
    // https://www.bing.com/webmasters
    public string? BingSiteVerification { get; init; }

    // https://search.google.com/
    public string? GoogleSiteVerification { get; init; }

    // Webmaster yandex: https://webmaster.yandex.kz/
    public string? YandexVerificationCode { get; init; }

    // Yandex Metrika tracking ID. Example: "12345678".
    public string? YandexMetrikaCode { get; init; }

    // List of domains to preconnect for faster resource loading.
    // Example: { "api.example.com", "cdn.example.com" }
    public string[]? PreconnectUrls { get; init; }

    // Google Fonts CSS URL. Example: "https://fonts.googleapis.com/css2?family=Roboto&display=swap"
    public string? GoogleFontsUrl { get; init; }

    // Google Analytics tracking ID. Example: "G-XXXXXXX".
    public string? GoogleAnalyticsCode { get; init; }

    public bool UseMessagePack { get; set; }

    public bool UseTelegram { get; set; }
}
