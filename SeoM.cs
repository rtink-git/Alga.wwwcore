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
    string? Robot = null, 
    string? UrlCanonical = null, 
    string? ImageUrl = null,
    int? ImageWidth = 0,
    int? ImageHeight = 0,
    string? Lang = null,
    DateTime? DatePublished = null // Date the page was published. Useful for news and blog sites
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

        string? imageFullUrl = null; 
        if(!string.IsNullOrWhiteSpace(this.ImageUrl))
            imageFullUrl = (this.ImageUrl[0] == '/') ? config.Url + ImageUrl : ImageUrl;

        // -- Open Graph meta tags

        AddPropMetaTag(html, "og:title", Title);
        AddPropMetaTag(html, "og:type", "article");
        AddPropMetaTag(html, "og:url", Url);
        AddPropMetaTag(html, "og:description", Description);
        AddPropMetaTag(html, "og:site_name", config.Name);
        AddPropMetaTag(html, "og:locale", config.Lang);
        if(imageFullUrl != null) {
            AddPropMetaTag(html, "og:image", imageFullUrl);
            AddPropMetaTag(html, "og:image:alt", $"{Title} Image");
            //AddPropMetaTag(html, "og:image:type", "image/jpeg");
            if(this.ImageWidth > 0 && this.ImageHeight > 0) {
                AddPropMetaTag(html, "og:image:width", ImageWidth.ToString());
                AddPropMetaTag(html, "og:image:height", ImageHeight.ToString());
            }
        }
    
        // -- Twitter meta tags

        AddMetaTag(html, "twitter:card", "summary_large_image");
        AddMetaTag(html, "twitter:title", Title);
        AddMetaTag(html, "twitter:url", Url);
        AddMetaTag(html, "twitter:description", Description);
        AddMetaTag(html, "twitter:site", config.TwitterSite);
        if(imageFullUrl != null) {
            AddMetaTag(html, "twitter:image", imageFullUrl);
            AddMetaTag(html, "twitter:image:alt", $"{Title} Image");
            if(this.ImageWidth > 0 && this.ImageHeight > 0) {
                AddMetaTag(html, "og:image:width", ImageWidth.ToString());
                AddMetaTag(html, "og:image:height", ImageHeight.ToString());
            }
        }

        // -- JSON-LD

        var typeJsonLD = (Url.TrimEnd('/') == config.Url) ? "WebSite" : "WebPage";

        html.Append("<script type=\"application/ld+json\">{");
        html.Append("\"@context\": \"https://schema.org\",");
        html.Append($"\"@type\": \"{typeJsonLD}\",");
        html.Append($"\"@url\": \"{Url}\",");
        html.Append($"\"@name\": \"{Title}\"");
        if(!string.IsNullOrWhiteSpace(Description))
            html.Append($",\"@description\": \"{Description}\"");
        if(DatePublished != null)
            html.Append($",\"@datePublished\": \"{DatePublished?.ToString("yyyy-MM-dd")}\"");
        if(!string.IsNullOrEmpty(this.ImageUrl) && this.ImageWidth > 0 && this.ImageHeight > 0)  {
            html.Append(",\"image\": {");
            html.Append("\"@type\": \"ImageObject\",");
            html.Append($"\"url\": \"{ImageUrl}\",");
            html.Append($"\"width\": \"{this.ImageWidth}\",");
            html.Append($"\"height\": \"{this.ImageHeight}\",");
            html.Append($"\"caption\": \"{Title} Image\"");
            html.Append("}");
        }
        html.Append("}</script>");

        return html.ToString();
    }

    /// <summary>
    /// Helper method that adds a meta tag to the HTML string if the content is not null or empty.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <param name="content"></param>
    void AddMetaTag(StringBuilder html, string name, string? content) { if (!string.IsNullOrEmpty(content)) html.Append($"<meta name=\"{name}\" content=\"{content}\">"); }

    void AddPropMetaTag(StringBuilder html, string property, string? content) { if (!string.IsNullOrEmpty(content)) html.Append($"<meta property=\"{property}\" content=\"{content}\">"); }

    /// <summary>
    /// Helper method that adds image-related meta tags (e.g., for Open Graph and Twitter).
    /// </summary>
    /// <param name="html"></param>
    /// <param name="prefix"></param>
    /// <param name="imageUrl"></param>
    /// <param name="title"></param>
    /// <param name="baseUrl"></param>
    // void AddImageMetaTag(StringBuilder html, string prefix, string? imageUrl, string title, string baseUrl = "") {
    //     if (!string.IsNullOrEmpty(imageUrl)) {
    //         var iur = (imageUrl[0] == '/') ? baseUrl + imageUrl : imageUrl;

    //         html.Append($"<meta property=\"{prefix}:image\" content=\"{iur}\">");
    //         html.Append($"<meta property=\"{prefix}:image:alt\" content=\"{title} Image\">");
    //         //<meta property="og:image:type" content="image/jpeg">
    //         if(this.ImageWidth > 0 && this.ImageHeight > 0) {
    //             html.Append($"<meta property=\"{prefix}:image:width\" content=\"{this.ImageWidth}\">");
    //             html.Append($"<meta property=\"{prefix}:image:height\" content=\"{this.ImageHeight}\">");
    //         }
    //     }
    // }

    // void AddLdJson(StringBuilder html, string name, string url, string? description, string baseUrl = "") {
    //     var type = (url.TrimEnd('/') == baseUrl) ? "WebSite" : "WebPage";

    //     if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url)) {
    //         html.Append("<script type=\"application/ld+json\">{");
    //         html.Append("\"@context\": \"https://schema.org\",");
    //         html.Append($"\"@type\": \"{type}\",");
    //         html.Append($"\"@url\": \"{url}\",");
    //         html.Append("\"@name\": \"" + name + "\"");
    //         if(!string.IsNullOrWhiteSpace(description))
    //             html.Append(",\"@description\": \"" + description + "\"");
    //         if(!string.IsNullOrEmpty(this.ImageUrl) && this.ImageWidth > 0 && this.ImageHeight > 0)  {
    //             html.Append(",\"image\": {");
    //             html.Append("\"@type\": \"ImageObject\",");
    //             html.Append($"\"url\": \"{this.ImageUrl}\",");
    //             html.Append($"\"width\": \"{this.ImageWidth}\",");
    //             html.Append($"\"height\": \"{this.ImageHeight}\",");
    //             html.Append($"\"caption\": \"{name} Image\"");
    //             html.Append("}");
    //         }
    //         html.Append("}</script>");
    //     }
    // }
};