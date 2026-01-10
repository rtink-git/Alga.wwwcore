using System.Text;
using System.Runtime.CompilerServices;

namespace Alga.wwwcore.Core.HtmlGenerator.SeoMetaGenerator;

sealed class Builder
{
    StringBuilder _outputSb;
    public Builder(StringBuilder outputSb) => _outputSb = outputSb;

    public void Do(Req req)
    {
        var spo = req.SeoPageOptions;

        // robots

        AppendMeta("robots", spo.Robot ?? "noindex, nofollow");

        // canonical

        var url = $"{req.Url}{spo.Path}";

        _outputSb.Append($"<link rel=\"canonical\" href=\"{req.Url}{spo.UrlCanonical}\" />");

        if (string.IsNullOrEmpty(spo.Title)) return;

        _outputSb.Append($"<title>{spo.Title}</title>");
        AppendMeta("description", spo.Description);

        // The Open Graph protocol. https://ogp.me

        AppendOG("og:type", spo.TypeOg);
        AppendOG("og:url", url);
        AppendOG("og:title", spo.Title);
        AppendOG("og:description", spo.Description);
        AppendOG("og:site_name", req.NameShort);
        AppendOG("og:locale", spo.Lang);

        AppendOG("product:price:amount", spo.ItemPrice?.ToString().Replace(",", "."));
        AppendOG("product:price:currency", spo.ItemCurrency);
        AppendOG("product:availability", spo.ItemAvailability);

        AppendOG("og:image", spo.ImageUrl);
        AppendOG("og:image:type", spo.ImageEncodingFormat);
        AppendOG("og:image:alt", spo.Title);
        if (spo.ImageWidth > 0 && spo.ImageHeight > 0)
        {
            AppendOG("og:image:width", spo.ImageWidth.ToString());
            AppendOG("og:image:height", spo.ImageHeight.ToString());
        }

        // Twitter

        if (!string.IsNullOrEmpty(req.TwitterSite))
        {
            AppendMeta("twitter:card", "summary_large_image");
            AppendMeta("twitter:url", url);
            AppendMeta("twitter:title", spo.Title);
            AppendMeta("twitter:description", spo.Description);
            AppendMeta("twitter:site", req.TwitterSite);
            AppendMeta("twitter:image", spo.ImageUrl);
            AppendMeta("twitter:image:alt", spo.Title);
        }

        // --- JSON-LD ---

        if (spo.SchemaOrgsJson?.Length > 32) _outputSb.Append($"<script type=\"application/ld+json\">{spo.SchemaOrgsJson}</script>");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendMeta(string name, string? content)
    {
        if (string.IsNullOrEmpty(content)) return;

        _outputSb.Append("<meta name=\"");
        _outputSb.Append(name);
        _outputSb.Append("\" content=\"");
        _outputSb.Append(content);
        _outputSb.Append("\">");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendOG(string property, string? content)
    {
        if (string.IsNullOrEmpty(content)) return;

        _outputSb.Append("<meta property=\"");
        _outputSb.Append(property);
        _outputSb.Append("\" content=\"");
        _outputSb.Append(content);
        _outputSb.Append("\">");
    }
}