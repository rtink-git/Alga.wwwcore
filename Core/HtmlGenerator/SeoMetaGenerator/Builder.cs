using System.IO.Pipelines;
using System.Text;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Alga.wwwcore.Core.HtmlGenerator.SeoMetaGenerator;

sealed class Builder
{
    static readonly Encoding Utf8 = Encoding.UTF8;
    static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    static readonly byte[] MetaNamePrefix = Encoding.UTF8.GetBytes("<meta name=\"");

    public void Do(Req req, PipeWriter writer)
    {
        var spo = req.SeoPageOptions;

        // robots

        WriteMeta("robots", spo.Robot);

        // canonical

        if (spo.UrlCanonical != null)
        {
            WriteString("<link rel=\"canonical\" href=\"");
            WriteString(req.Url);
            WriteString(spo.UrlCanonical);
            WriteString("\" />");
        }

        if (string.IsNullOrEmpty(spo.Title)) return;

        WriteString("<title>");
        WriteString(spo.Title);
        WriteString("</title>");

        WriteMeta("description", spo.Description);

        // The Open Graph protocol. https://ogp.me

        WriteOG("og:type", spo.TypeOg);
        if (!string.IsNullOrEmpty(req.Url) || !string.IsNullOrEmpty(spo.Path)) WriteOGUrl(req.Url, spo.Path);
        WriteOG("og:title", spo.Title);
        WriteOG("og:description", spo.Description);
        WriteOG("og:site_name", req.NameShort);
        WriteOG("og:locale", spo.Lang);
        if (spo.ItemPrice.HasValue) WriteOG("product:price:amount", spo.ItemPrice.Value.ToString(Invariant));
        WriteOG("product:price:currency", spo.ItemCurrency);
        WriteOG("product:availability", spo.ItemAvailability);

        WriteOG("og:image", spo.ImageUrl);
        WriteOG("og:image:type", spo.ImageEncodingFormat);
        WriteOG("og:image:alt", spo.Title);
        if (spo.ImageWidth > 0 && spo.ImageHeight > 0)
        {
            WriteOG("og:image:width", spo.ImageWidth.ToString());
            WriteOG("og:image:height", spo.ImageHeight.ToString());
        }

        // Twitter

        if (!string.IsNullOrEmpty(req.TwitterSite))
        {
            WriteMeta("twitter:card", "summary_large_image");
            WriteMetaUrl("twitter:url", req.Url, spo.Path);
            WriteMeta("twitter:title", spo.Title);
            WriteMeta("twitter:description", spo.Description);
            WriteMeta("twitter:site", req.TwitterSite);
            WriteMeta("twitter:image", spo.ImageUrl);
            WriteMeta("twitter:image:alt", spo.Title);
        }

        // --- JSON-LD ---

        if (spo.SchemaOrgsJson?.Length > 32)
        {
            WriteString("<script type=\"application/ld+json\">");
            WriteString(spo.SchemaOrgsJson);
            WriteString("</script>");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteMeta(string name, string? content)
        {
            if (string.IsNullOrEmpty(content)) return;

            WriteString("<meta name=\"");
            WriteString(name);
            WriteString("\" content=\"");
            WriteString(content);
            WriteString("\">");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteOG(string property, string? content)
        {
            if (string.IsNullOrEmpty(content)) return;

            WriteString("<meta property=\"");
            WriteString(property);
            WriteString("\" content=\"");
            WriteString(content);
            WriteString("\">");
        }

        // og:url = baseUrl + path (без строки-склейки)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteOGUrl(string? baseUrl, string? path)
        {
            if (string.IsNullOrEmpty(baseUrl) && string.IsNullOrEmpty(path))
                return;

            WriteString("<meta property=\"og:url\" content=\"");

            if (!string.IsNullOrEmpty(baseUrl))
                WriteString(baseUrl);

            if (!string.IsNullOrEmpty(path))
                WriteString(path);

            WriteString("\">");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteMetaUrl(string name, string? baseUrl, string? path)
        {
            if (string.IsNullOrEmpty(baseUrl) && string.IsNullOrEmpty(path))
                return;

            WriteString("<meta name=\"");
            WriteString(name);
            WriteString("\" content=\"");

            if (!string.IsNullOrEmpty(baseUrl))
                WriteString(baseUrl);

            if (!string.IsNullOrEmpty(path))
                WriteString(path);

            WriteString("\">");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteString(string s)
        {
            int byteCount = Utf8.GetByteCount(s);
            Span<byte> span = writer.GetSpan(byteCount);
            int written = Utf8.GetBytes(s, span);
            writer.Advance(written);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteSpan(ReadOnlySpan<char> s)
        {
            int byteCount = Utf8.GetByteCount(s);
            Span<byte> span = writer.GetSpan(byteCount);
            int written = Utf8.GetBytes(s, span);
            writer.Advance(written);
        }
    }
}