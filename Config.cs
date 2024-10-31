namespace Alga.wwwcore;

public static class Config
{
    public static bool IsDebug { get; set;} = false;
    public static string? name { get; set; } = null;
    public static string? url { get; set; } = null;
    public static string? WebRootUrl { get; set; } = null;
    public static string? WebRootUrlPart_Activities { get; set; } = "/Activities";
    public static string? WebRootUrlPart_Activities_Components { get; set; } = "/Activities/Components";
    public static string? WebRootUrlPart_Activities_ExternalComponents { get; set; } = "/Activities/ExternalComponents";
    public static string? WebRootUrlPart_Activities_UIRs { get; set; } = "/Activities/UIRs";
    public static List<string>? PreconnectUrls { get; set; } = null;
  
    /// <summary>
    /// Project Icon 32x32 px.
    /// Recomended url (is default): "/Activities/Components/Total/content/Icon-32.png"
    /// </summary>
    public static string? Icon32Url { get; set; } = "/Activities/Components/Total/content/Icon-32.png";
    /// <summary>
    /// Project Icon 180x180 px.
    /// Recomended url (is default): "/Activities/Components/Total/content/Icon-180.png"
    /// </summary>
    public static string? Icon180Url { get; set; } = "/Activities/Components/Total/content/Icon-180.png";
    /// <summary>
    /// Project Icon 192x192 px.
    /// Recomended url (is default): "/Activities/Components/Total/content/Icon-192.png"
    /// </summary>
    public static string? Icon192Url { get; set; } = "/Activities/Components/Total/content/Icon-192.png";
    /// <summary>
    /// Project Icon 512x512 px.
    /// Recomended url (is default): "/Activities/Components/Total/content/Icon-512.png"
    /// </summary>
    public static string? Icon512Url { get; set; } = "/Activities/Components/Total/content/Icon-512.png";
    /// <summary>
    /// Google Fonts 
    /// https://fonts.google.com/
    /// </summary>
    public static string? GoogleFontsUrl { get; set; } = null;
    /// <summary>
    /// Google Analytics Code
    /// Example: G-9DDDCCTTXX
    /// </summary>
    public static string? GoogleAnalyticsCode { get; set; } = null;
    /// <summary>
    /// Yandex Metrika Code
    /// Example: 99991111
    /// </summary>
    public static string? YandexMetrikaCode { get; set; } = null;
    public static string? TwitterSite { get; set; } = null; // example: @ElonMusk
}
