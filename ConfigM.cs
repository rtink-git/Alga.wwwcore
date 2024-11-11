namespace Alga.wwwcore;

public record ConfigM (
    /// <summary>
    /// Build Mode
    /// </summary>
    bool IsDebug,
    /// <summary>
    /// Url of your application
    /// Example: (IsDebug) ? "https://localhost:1234" : "https://example.com"
    /// </summary>
    string Url,
    /// <summary>
    /// Name of your application
    /// Example: My Awesome App
    /// </summary>
    string Name,
    /// <summary>
    /// Short name of your application
    /// Example: AwesomeApp
    /// </summary>
    string NameShort,
    /// <summary>
    /// Brief description of what your application does
    /// Example: A great app for managing your tasks.
    /// </summary>
    string Description,
    /// <summary>
    /// </summary>
    int CacheControlInSDefault = -1,
    /// <summary>
    /// Preconnect urls
    /// Example: new List<string>() { api.example.com, api1.example.com, api2.example.com }
    /// </summary>
    List<string>? PreconnectUrls = null,
    /// <summary>
    /// Google Fonts 
    /// https://fonts.google.com/
    /// </summary>
    string? GoogleFontsUrl = null,
    /// <summary>
    /// Google Analytics Code
    /// Example: G-9DDDCCTTXX
    /// </summary>
    string? GoogleAnalyticsCode = null,
    /// <summary>
    /// Yandex Metrika Code
    /// Example: 99991111
    /// </summary>
    string? YandexMetrikaCode = null,
    /// <summary>
    /// Twitter account name
    /// Example: @ElonMusk
    /// </summary>
    string? TwitterSite = null,
    /// <summary>
    /// The background color that will be used when the application loads. Necessary for PWA to work correctly
    /// Example: #FFFFFF | #000000
    /// </summary>
    string BackgroundColor = "#FFFFFF",
    /// <summary>
    /// Defines the color of the interface and visual elements of your PWA
    /// Example: #FFFFFF | #000000
    /// </summary>
    string ThemeColor = "#FFFFFF"
);
