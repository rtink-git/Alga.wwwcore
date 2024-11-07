using System;
using System.Drawing;

namespace Alga.wwwcore;

public class ConfigModel
{
    /// <summary>
    /// Name of your application
    /// Example: My Awesome App
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Short name of your application
    /// Example: AwesomeApp
    /// </summary>
    public string? NameShort { get; set; }
    /// <summary>
    /// Brief description of what your application does
    /// Example: A great app for managing your tasks.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Url of your application
    /// Example: (IsDebug) ? "https://localhost:1234" : "https://example.com"
    /// </summary>
    public string? Url { get; set; }
    /// <summary>
    /// The background color that will be used when the application loads. Necessary for PWA to work correctly
    /// Example: #FFFFFF | #000000
    /// </summary>
    public string BackgroundColor { get; set; } = "#FFFFFF";
    /// <summary>
    /// Defines the color of the interface and visual elements of your PWA
    /// Example: #FFFFFF | #000000
    /// </summary>
    public string ThemeColor { get; set; } = "#FFFFFF";

    public int UICacheControlInSDefault { get; set; } = -1;

    /// <summary>
    /// Preconnect urls
    /// Example: new List<string>() { api.example.com, api1.example.com, api2.example.com }
    /// </summary>
    public List<string>? PreconnectUrls  { get; set; }
    /// <summary>
    /// Google Fonts 
    /// https://fonts.google.com/
    /// </summary>
    public string? GoogleFontsUrl { get; set; }
    /// <summary>
    /// Google Analytics Code
    /// Example: G-9DDDCCTTXX
    /// </summary>
    public string? GoogleAnalyticsCode { get; set; }
    /// <summary>
    /// Yandex Metrika Code
    /// Example: 99991111
    /// </summary>
    public string? YandexMetrikaCode { get; set; }
    /// <summary>
    /// Twitter account name
    /// Example: @ElonMusk
    /// </summary>
    public string? TwitterSite { get; set; }
}
