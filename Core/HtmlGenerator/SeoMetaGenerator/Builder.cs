using System.Globalization;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using System.Buffers;

namespace Alga.wwwcore.Core.HtmlGenerator.SeoMetaGenerator;

sealed class Builder
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static readonly Encoding Utf8Enc = Encoding.UTF8;
    static ReadOnlySpan<byte> MetaName => "<meta name=\""u8;
    static ReadOnlySpan<byte> MetaProp => "<meta property=\""u8;
    static ReadOnlySpan<byte> MetaContent => "\" content=\""u8;
    static ReadOnlySpan<byte> MetaClose => "\">"u8;
    static ReadOnlySpan<byte> LinkCanonical => "<link rel=\"canonical\" href=\""u8;
    static ReadOnlySpan<byte> TitleOpen => "<title>"u8;
    static ReadOnlySpan<byte> TitleClose => "</title>"u8;
    static ReadOnlySpan<byte> OgUrl => "<meta property=\"og:url\" content=\""u8;
    static ReadOnlySpan<byte> TwitterCard => "<meta name=\"twitter:card\" content=\"summary_large_image\">"u8;
    static ReadOnlySpan<byte> JsonLdOpen => "<script type=\"application/ld+json\">"u8;
    static ReadOnlySpan<byte> JsonLdClose => "</script>"u8;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Write(Req req, PipeWriter writer)
    {
        var spo = req.SeoPageOptions;
        if (string.IsNullOrEmpty(spo.Title)) return;

        var w = new Utf8BufferWriter(writer);

        // robots
        WriteMetaNameContent(ref w, "robots"u8, spo.Robot);

        // canonical
        if (spo.UrlCanonical != null)
        {
            w.Write(LinkCanonical);
            w.Write(req.Url);
            w.Write(spo.UrlCanonical);
            w.WriteByte((byte)'"');
            w.WriteByte((byte)'>');
        }

        // title
        w.Write(TitleOpen);
        w.Write(spo.Title);
        w.Write(TitleClose);

        // description
        WriteMetaNameContent(ref w, "description"u8, spo.Description);

        // Open Graph

        WriteMetaPropContent(ref w, "og:type"u8, spo.TypeOg);
        WriteOgUrl(ref w, req.Url, spo.Path);
        WriteMetaPropContent(ref w, "og:title"u8, spo.Title);
        WriteMetaPropContent(ref w, "og:description"u8, spo.Description);
        WriteMetaPropContent(ref w, "og:site_name"u8, req.NameShort);
        WriteMetaPropContent(ref w, "og:locale"u8, spo.Lang);

        if (spo.ItemPrice.HasValue)
            WriteMetaPropContent(ref w, "product:price:amount"u8, spo.ItemPrice.Value.ToString(Inv));

        WriteMetaPropContent(ref w, "product:price:currency"u8, spo.ItemCurrency);
        WriteMetaPropContent(ref w, "product:availability"u8, spo.ItemAvailability);

        WriteMetaPropContent(ref w, "og:image"u8, spo.ImageUrl);
        WriteMetaPropContent(ref w, "og:image:type"u8, spo.ImageEncodingFormat);
        WriteMetaPropContent(ref w, "og:image:alt"u8, spo.Title);

        if (spo.ImageWidth > 0 && spo.ImageHeight > 0)
        {
            WriteMetaPropContent(ref w, "og:image:width"u8, spo.ImageWidth.ToString());
            WriteMetaPropContent(ref w, "og:image:height"u8, spo.ImageHeight.ToString());
        }

        // Twitter
        if (!string.IsNullOrEmpty(req.TwitterSite))
        {
            w.Write(TwitterCard);
            WriteMetaNameUrl(ref w, "twitter:url"u8, req.Url, spo.Path);
            WriteMetaNameContent(ref w, "twitter:title"u8, spo.Title);
            WriteMetaNameContent(ref w, "twitter:description"u8, spo.Description);
            WriteMetaNameContent(ref w, "twitter:site"u8, req.TwitterSite);
            WriteMetaNameContent(ref w, "twitter:image"u8, spo.ImageUrl);
            WriteMetaNameContent(ref w, "twitter:image:alt"u8, spo.Title);
        }

        // JSON-LD
        var json = spo.SchemaOrgsJson;
        if (!string.IsNullOrEmpty(json) && json.Length > 32)
        {
            w.Write(JsonLdOpen);
            w.Write(json);
            w.Write(JsonLdClose);
        }

        // Можно не вызывать FlushAsync здесь, если вызывающий код сам управляет
        // w.Commit();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void WriteMetaNameContent(ref Utf8BufferWriter w, ReadOnlySpan<byte> name, string? value)
    {
        if (string.IsNullOrEmpty(value)) return;
        w.Write(MetaName);
        w.Write(name);
        w.Write(MetaContent);
        w.Write(value);
        w.Write(MetaClose);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteMetaNameUrl(ref Utf8BufferWriter w, ReadOnlySpan<byte> name, string? baseUrl, string? path)
    {
        if (string.IsNullOrEmpty(baseUrl) && string.IsNullOrEmpty(path)) return;
        w.Write(MetaName);
        w.Write(name);
        w.Write(MetaContent);
        if (baseUrl != null) w.Write(baseUrl);
        if (path != null) w.Write(path);
        w.Write(MetaClose);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteMetaPropContent(ref Utf8BufferWriter w, ReadOnlySpan<byte> prop, string? value)
    {
        if (string.IsNullOrEmpty(value)) return;
        w.Write(MetaProp);
        w.Write(prop);
        w.Write(MetaContent);
        w.Write(value);
        w.Write(MetaClose);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteOgUrl(ref Utf8BufferWriter w, string? baseUrl, string? path)
    {
        if (string.IsNullOrEmpty(baseUrl) && string.IsNullOrEmpty(path)) return;
        w.Write(OgUrl);
        if (baseUrl != null) w.Write(baseUrl);
        if (path != null) w.Write(path);
        w.Write(MetaClose);
    }

    readonly ref struct Utf8BufferWriter
    {
        private readonly PipeWriter _writer;

        public Utf8BufferWriter(PipeWriter writer) => _writer = writer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<byte> data)
        {
            var span = _writer.GetSpan(data.Length);
            data.CopyTo(span);
            _writer.Advance(data.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string? s)
        {
            if (s == null) return;

            var byteCount = Utf8Enc.GetByteCount(s);
            var span = _writer.GetSpan(byteCount);
            Utf8Enc.GetBytes(s, span);
            _writer.Advance(byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte b)
        {
            var span = _writer.GetSpan(1);
            span[0] = b;
            _writer.Advance(1);
        }

        // public void Commit() => _writer.FlushAsync();  // раскомментируй, если нужно
    }
}