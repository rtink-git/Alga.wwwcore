using System.Text;
using System.Runtime.CompilerServices;

namespace Alga.wwwcore;

// Immutable holder for SEO metadata that emits HTML tags & JSON-LD
public readonly record struct Seo(
    string Title,
    string Url,
    string? Description = null,
    string? Robot = null,
    string? UrlCanonical = null,
    string? ImageUrl = null,
    int? ImageWidth = 0,
    int? ImageHeight = 0,
    string? Lang = null,
    DateTime? DatePublished = null)
{
    private const int JsonLdCapacity = 512;
    
    // Generates structured markup for meta tags, Open Graph, Twitter Cards, and JSON-LD schema
    internal string MergeTags(Models.Config config)
    {
        var sb = new StringBuilder(2048);

        // robots
        AppendMeta(sb, "robots", Robot);

        // canonical
        if (!string.IsNullOrEmpty(UrlCanonical))
            sb.Append($"<link rel=\"canonical\" href=\"{config.Url}{UrlCanonical}\" />");

        // title
        if (!string.IsNullOrEmpty(Title))
            sb.Append("<title>").Append(Title).Append("</title>");

        // description
        AppendMeta(sb, "description", Description);

        // Build full image URL once
        string? imgFull = ImageUrl is { Length: >0 } img && img[0] == '/'
            ? string.Concat(config.Url, img)
            : ImageUrl;

        // — Open Graph —
        AppendOG(sb, "article", "og:type");
        AppendOG(sb, Url, "og:url");
        AppendOG(sb, Title, "og:title");
        AppendOG(sb, Description, "og:description");
        AppendOG(sb, config.Name, "og:site_name");
        AppendOG(sb, config.Lang ?? Lang, "og:locale");
        
        if (imgFull is not null)
        {
            AppendOG(sb, imgFull, "og:image");
            AppendOG(sb, $"{Title} Image", "og:image:alt");
            if (ImageWidth is >0 and int w && ImageHeight is >0 and int h)
            {
                AppendOG(sb, w.ToString(), "og:image:width");
                AppendOG(sb, h.ToString(), "og:image:height");
            }
        }

        // — Twitter —
        AppendMeta(sb, "twitter:card", "summary_large_image");
        AppendMeta(sb, "twitter:title", Title);
        AppendMeta(sb, "twitter:url", Url);
        AppendMeta(sb, "twitter:description", Description);
        AppendMeta(sb, "twitter:site", config.TwitterSite);
        
        if (imgFull is not null)
        {
            AppendMeta(sb, "twitter:image", imgFull);
            AppendMeta(sb, "twitter:image:alt", $"{Title} Image");
        }

        // — JSON-LD —
        string typeJsonLD = Url.AsSpan().TrimEnd('/').SequenceEqual(config.Url.AsSpan())
            ? "WebSite"
            : "WebPage";

        var jsonSb = new StringBuilder(JsonLdCapacity);
        jsonSb.Append("<script type=\"application/ld+json\">{\"@context\":\"https://schema.org\",");
        jsonSb.Append("\"@type\":\"").Append(typeJsonLD).Append("\",");
        jsonSb.Append("\"url\":\"").Append(Url).Append("\",");
        jsonSb.Append("\"name\":\"").Append(Title).Append('\"');
        
        if (!string.IsNullOrEmpty(Description))
            jsonSb.Append(",\"description\":\"").Append(Description).Append('\"');
        
        if (DatePublished is DateTime date)
            jsonSb.Append(",\"datePublished\":\"").Append(date.ToString("yyyy-MM-dd")).Append('\"');
        
        if (imgFull is not null && ImageWidth > 0 && ImageHeight > 0)
        {
            jsonSb.Append(",\"image\":{\"@type\":\"ImageObject\",\"url\":\"")
                .Append(imgFull)
                .Append("\",\"width\":\"")
                .Append(ImageWidth.Value)
                .Append("\",\"height\":\"")
                .Append(ImageHeight.Value)
                .Append("\",\"caption\":\"")
                .Append(Title)
                .Append(" Image\"}");
        }
        
        jsonSb.Append("}</script>");
        sb.Append(jsonSb);

        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendMeta(StringBuilder sb, string name, string? content)
    {
        if (!string.IsNullOrEmpty(content))
            sb.Append("<meta name=\"").Append(name).Append("\" content=\"").Append(content).Append("\">");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendOG(StringBuilder sb, string? content, string property)
    {
        if (!string.IsNullOrEmpty(content))
            sb.Append("<meta property=\"").Append(property).Append("\" content=\"").Append(content).Append("\">");
    }
}