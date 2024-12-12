using System.Dynamic;

namespace Alga.wwwcore;

/// <summary>
/// 
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
public class ConfigM {
    public bool IsDebug {get; set;}
    public string Url {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public string NameShort {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public int CacheControlInSDefault  {get; set;}
    public List<string>? PreconnectUrls  {get; set;}
    public string? GoogleFontsUrl  {get; set;}
    public string? GoogleAnalyticsCode  {get; set;}
    public string? YandexMetrikaCode  {get; set;}
    public string? TwitterSite  {get; set;}
    public string BackgroundColor  {get; set;} = "#FFFFFF";
    public string ThemeColor  {get; set;} = "#FFFFFF";
};
