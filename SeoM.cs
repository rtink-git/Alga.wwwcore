using System.Text;

namespace Alga.wwwcore;

public record SeoM (
    string Title, 
    string? Description = null, 
    string? Robot = null, string? 
    UrlCanonical = null, string? 
    ImageUrl = null
) {
    internal string MergeTags(ConfigM config) {
        var html = new StringBuilder();

        //-- base settings

        AddMetaTag(html, "robots", Robot);
        AddMetaTag(html, "description", Description);
        if (!string.IsNullOrEmpty(UrlCanonical)) html.AppendFormat("<meta rel=\"canonical\" href=\"{0}{1}\">", config.Url, UrlCanonical);

        if (string.IsNullOrEmpty(Title)) return html.ToString();

        html.AppendFormat("<title>{0}</title>", Title);

        // --- Open Graph meta tags ---
        if (!string.IsNullOrEmpty(config.Name))
        {
            html.AppendFormat("<meta property=\"og:site_name\" content=\"{0}\">", config.Name);
            html.Append("<meta property=\"og:type\" content=\"article\">");
            html.AppendFormat("<meta property=\"og:url\" content=\"{0}\">", config.Url);
            html.AppendFormat("<meta property=\"og:title\" content=\"{0}\">", Title);
            AddMetaTag(html, "og:description", Description);
            AddImageMetaTag(html, "og", ImageUrl, Title, config.Url);
        }
    
        // --- Twitter meta tags ---
        if (!string.IsNullOrEmpty(config.TwitterSite))
        {
            html.AppendFormat("<meta name=\"twitter:site\" content=\"{0}\">", config.TwitterSite);
            html.Append("<meta name=\"twitter:card\" content=\"summary_large_image\">");
            html.Append("<meta name=\"twitter:description\" content=\"summary_large_image\">");
            html.AppendFormat("<meta name=\"twitter:title\" content=\"{0}\">", Title);
            AddMetaTag(html, "twitter:description", Description);
            AddImageMetaTag(html, "twitter", ImageUrl, Title);
        }

        return html.ToString();
    }

    void AddMetaTag(StringBuilder html, string name, string? content)
    {
        if (!string.IsNullOrEmpty(content))
            html.AppendFormat("<meta name=\"{0}\" content=\"{1}\">", name, content);
    }

    void AddImageMetaTag(StringBuilder html, string prefix, string? imageUrl, string title, string baseUrl = "")
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            html.AppendFormat("<meta property=\"{0}:image\" content=\"{1}{2}\">", prefix, baseUrl, imageUrl);
            html.AppendFormat("<meta property=\"{0}:image:alt\" content=\"{1}\">", prefix, title);
        }
    }
};