using Microsoft.Extensions.Logging;

namespace Alga.wwwcore;

/// <summary>
/// Generates a User Interface diagram for sending HTTP responses.
/// </summary>
public class Root {

    /// <summary>
    /// The configuration manager used to retrieve App configuration.
    /// </summary>
    readonly ConfigModel ConfigM;
    //readonly IHttpContextAccessor HttpContextAccessor;
    readonly ILogger? _Logger;

    /// <summary>
    /// Initializes a new instance of the "Root" class
    /// </summary>
    /// <param name="config">A configuration object containing general App settings.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is null.</exception>
    public Root(ConfigModel config, ILogger? logger = null) { // , ILoggerFactory? loggerFactory = null
        ConfigM = config ?? throw new ArgumentNullException(nameof(config));
        _Logger = logger;
    }

    public async Task Start() {
        _Logger?.LogInformation("Alga.wwwcore nuget paackage - Started");

        var schemes = new _Schemes(_Logger);
        await schemes.Build();
        await new _Index(ConfigM, schemes.List, schemes.Modules, _Logger).Build();
        new _ManifestJson(this.ConfigM).Build();
        new _AppJs(ConfigM).Create();
        new _ServiceworkerJs().Build(ConfigM, schemes.List, schemes.Modules);
    }



    /// <summary>
    /// Sends an HTTP response asynchronously with the specified UI screen scheme and optional SEO metadata.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
    /// <param name="UISName">The name of the UI screen scheme to be used for rendering the response.</param>
    /// <param name="seoM">Optional SEO metadata to include in the response.</param>
    /// <param name="cacheControlInS">Optional cache control setting (in seconds). Default is -1, in them mens but better add cache control. The default cache control setting (in seconds) for the entire project can be set in the configuration when initializing the class.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    //public async Task SendAsync(string UISName, SeoM? seoM = null, int cacheControlInS = -1) => await new _Response(ConfigM, UISName).Send(HttpContextAccessor.HttpContext, seoM, cacheControlInS);

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
    public class ConfigModel {
        internal string Url {get; set;} = string.Empty;
        public bool IsDebug {get; set;}
        public string Name {get; set;} = string.Empty;
        public string NameShort {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public int CacheControlInSDefault {get; set;}
        public List<string>? PreconnectUrls {get; set;}
        public string? GoogleFontsUrl {get; init;}
        public string? GoogleAnalyticsCode {get; init;}
        public string? YandexMetrikaCode {get; init;}
        public string? TwitterSite {get; init;}
        public string BackgroundColor {get; set;} = "#FFFFFF";
        public string ThemeColor {get; set;} = "#FFFFFF";
        public string Lang {get; set;}
        public string offlinePageUrl {get; set;} = "/offline";
        public string[] cacheUrls {get; set;}
    };
}