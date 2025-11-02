using System.Text;
using System.Runtime.CompilerServices;
using NUglify.JavaScript.Syntax;

namespace Alga.wwwcore;

// Immutable holder for SEO metadata that emits HTML tags & JSON-LD
public sealed class Seo
{
    StringBuilder _sb;
    Models.Config _config;
    Models.Seo _model;

    public Seo(StringBuilder sb, Models.Config config, Models.Seo model)
    {
        _sb = sb;
        _config = config;
        _model = model;
    }

    // Generates structured markup for meta tags, Open Graph, Twitter Cards, and JSON-LD schema
    internal void MergeTags()
    {
        // robots

        AppendMeta("robots", _model.Robot ?? "noindex, nofollow");

        // canonical

        var url = $"{_config.Url}{_model.Path}";

        if (_model.UrlCanonical != null && _model.UrlCanonical.Replace(_config.Url, "") != _model.Path)
            _sb.Append($"<link rel=\"canonical\" href=\"{_config.Url}{_model.UrlCanonical}\" />");

        if (!string.IsNullOrEmpty(_model.Title))
        {
            _sb.Append($"<title>{_model.Title}</title>");
            AppendMeta("description", _model.Description);

            // The Open Graph protocol. https://ogp.me

            AppendOG("og:type", _model.TypeOg);
            AppendOG("og:url", url);
            AppendOG("og:title", _model.Title);
            AppendOG("og:description", _model.Description);
            AppendOG("og:site_name", _config.NameShort);
            AppendOG("og:locale", _config.Lang);

            AppendOG("product:price:amount", _model.ItemPrice?.ToString().Replace(",", "."));
            AppendOG("product:price:currency", _model.ItemCurrency);
            AppendOG("product:availability", _model.ItemAvailability);


            AppendOG("og:image", _model.ImageUrl);
            AppendOG("og:image:type", _model.ImageEncodingFormat);
            AppendOG("og:image:alt", _model.Title);
            if (_model.ImageWidth > 0 && _model.ImageHeight > 0)
            {
                AppendOG("og:image:width", _model.ImageWidth.ToString());
                AppendOG("og:image:height", _model.ImageHeight.ToString());
            }

            // Twitter

            AppendMeta("twitter:card", "summary_large_image");
            AppendMeta("twitter:url", url);
            AppendMeta("twitter:title", _model.Title);
            AppendMeta("twitter:description", _model.Description);
            AppendMeta("twitter:site", _config.TwitterSite);
            AppendMeta("twitter:image", _model.ImageUrl);
            AppendMeta("twitter:image:alt", _model.Title);

            // --- JSON-LD ---

            if (_model.SchemaOrgsJson?.Length > 32) _sb.Append($"<script type=\"application/ld+json\">{_model.SchemaOrgsJson}</script>");

        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendMeta(string name, string? content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            _sb.Append("<meta name=\"");
            _sb.Append(name);
            _sb.Append("\" content=\"");
            _sb.Append(content);
            _sb.Append("\">");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendOG(string property, string? content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            _sb.Append("<meta property=\"");
            _sb.Append(property);
            _sb.Append("\" content=\"");
            _sb.Append(content);
            _sb.Append("\">");
        }
    }
}