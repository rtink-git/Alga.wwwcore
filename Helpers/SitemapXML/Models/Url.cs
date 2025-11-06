namespace Alga.wwwcore.Helpers.SitemapXML.Models;

public sealed class Url
{
    public required string Loc { get; init; } // URL of the page. This URL must begin with the protocol (such as http) and end with a trailing slash, if your web server requires it. This value must be less than 2,048 characters.
    public DateTime? LastMod { get; init; } // The date of last modification of the page. This date should be in W3C Datetime format. This format allows you to omit the time portion, if desired, and use YYYY-MM-DD.
    public string? ChangeFreq { get; init; } // How frequently the page is likely to change.
    public double? Priority { get; init; } // The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0. This value does not affect how your pages are compared to pages on other sites—it only lets the search engines know which pages you deem most important for the crawlers.

    /// <summary>
    /// Alternative language versions of the page.
    /// Used to generate <xhtml:link rel="alternate" hreflang="..." href="..."/> tags in sitemap.xml.
    /// The key is the language code (ISO 639-1), and the value is the URL of the alternative page.
    /// </summary>
    /// <example>
    /// AlternateUrls = new Dictionary&lt;string, string&gt;
    /// {
    ///     { "en", "https://example.com/en/product/123" },
    ///     { "ru", "https://example.com/product/123" },
    ///     { "fr", "https://example.com/fr/product/123" }
    /// };
    /// </example>
    public Dictionary<string, string>? AlternateUrls { get; init; }
    public List<Image>? Images { get; init; }
    public Video? Video { get; init; }
    // public Product? Product { get; init; }
    public News? News { get; init; }
}
