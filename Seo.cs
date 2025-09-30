using System.Text;
using System.Runtime.CompilerServices;

namespace Alga.wwwcore;

// Immutable holder for SEO metadata that emits HTML tags & JSON-LD
public sealed class Seo(Models.Seo model)
{
    private const int JsonLdCapacity = 512;
    
    // Generates structured markup for meta tags, Open Graph, Twitter Cards, and JSON-LD schema
    internal string MergeTags(Models.Config config)
    {
        var sb = new StringBuilder(2048);

        // robots
        AppendMeta(sb, "robots", model.Robot ?? "noindex, nofollow");

        // canonical

        var url = $"{config.Url}{model.Path}";
        //var urlC = model.UrlCanonical == null ? url : $"{config.Url}{model.UrlCanonical}";

        if(model.UrlCanonical != null && model.UrlCanonical.Replace(config.Url, "") != model.Path)
            sb.Append($"<link rel=\"canonical\" href=\"{config.Url}{model.UrlCanonical}\" />");

        if (model.SchemaOrg != null)
        {

            // title
            if (!string.IsNullOrEmpty(model.Title))
                sb.Append("<title>").Append(model.Title).Append("</title>");

            // description
            if(!string.IsNullOrEmpty(model.SchemaOrg.Description)) AppendMeta(sb, "description", model.SchemaOrg.Description);

            // Build full image URL once
            string? imgFull = model.ImageUrl is { Length: > 0 } img && img[0] == '/'
                ? string.Concat(config.Url, img)
                : model.ImageUrl;

            // — Open Graph —
            AppendOG(sb, "article", "og:type");
            AppendOG(sb, url, "og:url");
            AppendOG(sb, model.Title, "og:title");
            if(!string.IsNullOrEmpty(model.SchemaOrg.Description)) AppendOG(sb, model.SchemaOrg.Description, "og:description");
            AppendOG(sb, config.Name, "og:site_name");
            AppendOG(sb, config.Lang ?? model.Lang, "og:locale");

            if (imgFull is not null)
            {
                AppendOG(sb, imgFull, "og:image");
                AppendOG(sb, $"{model.Title} Image", "og:image:alt");
                if (model.ImageWidth is > 0 and int w && model.ImageHeight is > 0 and int h)
                {
                    AppendOG(sb, w.ToString(), "og:image:width");
                    AppendOG(sb, h.ToString(), "og:image:height");
                }
            }

            // — Twitter —
            AppendMeta(sb, "twitter:card", "summary_large_image");
            AppendMeta(sb, "twitter:title", model.Title);
            AppendMeta(sb, "twitter:url", url);
            if(!string.IsNullOrEmpty(model.SchemaOrg.Description)) AppendMeta(sb, "twitter:description", model.SchemaOrg.Description);
            AppendMeta(sb, "twitter:site", config.TwitterSite);

            if (imgFull is not null)
            {
                AppendMeta(sb, "twitter:image", imgFull);
                AppendMeta(sb, "twitter:image:alt", $"{model.Title} Image");
            }

            sb.Append("<script type=\"application/ld+json\">");
            sb.Append(new SchemaOrg(model.SchemaOrg).GetJsonLD(url, model.Title, model.SchemaOrg.Description));
            sb.Append("</script>");
        }

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