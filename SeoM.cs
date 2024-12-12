using System.Text;

namespace Alga.wwwcore;

/// <summary>
/// A record that holds SEO metadata and generates the corresponding HTML meta tags.
/// </summary>
/// <param name="Title">The title of the page</param>
/// <param name="Description">A short description of the page content</param>
/// <param name="Robot">Robots meta tag (used for controlling search engine crawlers)</param>
/// <param name="UrlCanonical">The canonical URL for the page to avoid duplicate content.</param>
/// <param name="ImageUrl">The URL of an image to use for social media previews.</param>
public record SeoM (
    string Title, 
    string Url,
    string? Description = null, 
    string? Robot = null, string?
    UrlCanonical = null, string? 
    ImageUrl = null
) {
    /// <summary>
    /// Generates a string of HTML meta tags based on the SEO metadata in this record.
    /// </summary>
    /// <param name="config">The configuration object that holds site-specific data (e.g., URL, site name, Twitter handle).</param>
    /// <returns>A string of HTML meta tags for the SEO configuration.</returns>
    internal string MergeTags(ConfigM config) {
        var html = new StringBuilder();

        //-- base settings

        AddMetaTag(html, "robots", Robot);
        AddMetaTag(html, "description", Description);
        if (!string.IsNullOrEmpty(UrlCanonical)) html.Append($"<meta rel=\"canonical\" href=\"{config.Url}{UrlCanonical}\">");

        if (string.IsNullOrEmpty(Title)) return html.ToString();

        if (!string.IsNullOrEmpty(Title)) html.Append($"<title>{Title}</title>");

        // --- Open Graph meta tags ---
        if (!string.IsNullOrEmpty(config.Name))
        {
            html.Append($"<meta property=\"og:site_name\" content=\"{config.Name}\">");
            html.Append("<meta property=\"og:type\" content=\"article\">");
            html.Append($"<meta property=\"og:url\" content=\"{Url}\">");
            html.Append($"<meta property=\"og:title\" content=\"{Title}\">");
            AddMetaTag(html, "og:description", Description);
            AddImageMetaTag(html, "og", ImageUrl, Title, config.Url);
        }
    
        // --- Twitter meta tags ---
        if (!string.IsNullOrEmpty(config.TwitterSite))
        {
            html.Append($"<meta name=\"twitter:site\" content=\"{config.TwitterSite}\">");
            html.Append("<meta name=\"twitter:card\" content=\"summary_large_image\">");
            html.Append("<meta name=\"twitter:description\" content=\"summary_large_image\">");
            html.Append($"<meta name=\"twitter:title\" content=\"{Title}\">");
            AddMetaTag(html, "twitter:description", Description);
            AddImageMetaTag(html, "twitter", ImageUrl, Title);
        }

        return html.ToString();
    }

    /// <summary>
    /// Helper method that adds a meta tag to the HTML string if the content is not null or empty.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <param name="content"></param>
    void AddMetaTag(StringBuilder html, string name, string? content) { if (!string.IsNullOrEmpty(content)) html.Append($"<meta name=\"{name}\" content=\"{content}\">"); }

    /// <summary>
    /// Helper method that adds image-related meta tags (e.g., for Open Graph and Twitter).
    /// </summary>
    /// <param name="html"></param>
    /// <param name="prefix"></param>
    /// <param name="imageUrl"></param>
    /// <param name="title"></param>
    /// <param name="baseUrl"></param>
    void AddImageMetaTag(StringBuilder html, string prefix, string? imageUrl, string title, string baseUrl = "") {
        if (!string.IsNullOrEmpty(imageUrl)) {
            html.Append($"<meta property=\"{prefix}:image\" content=\"{baseUrl}{imageUrl}\">");
            html.Append($"<meta property=\"{prefix}:image:alt\" content=\"{title}\">");
        }
    }
};