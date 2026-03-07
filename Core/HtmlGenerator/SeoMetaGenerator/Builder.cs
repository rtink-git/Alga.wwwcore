using System.Globalization;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using System.Buffers.Text;

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
    static ReadOnlySpan<byte> LinkRelOpen => "<link rel=\""u8;
    static ReadOnlySpan<byte> LinkHref => "\" href=\""u8;
    static ReadOnlySpan<byte> LinkSizes => "\" sizes=\""u8;
    static ReadOnlySpan<byte> LinkTypeOpen => "\" type=\""u8;
    static ReadOnlySpan<byte> TagClose => "\">"u8;
    static ReadOnlySpan<byte> X => "x"u8;

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

        // icons
        WriteIcon(ref w, 32, spo.Icon32?.Url, spo.Icon32?.EncodingFormat, "icon"u8);
        WriteIcon(ref w, 180, spo.Icon180?.Url, spo.Icon180?.EncodingFormat, "apple-touch-icon"u8);

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

        WriteMetaPropContent(ref w, "og:image"u8, spo.Image?.Url);
        WriteMetaPropContent(ref w, "og:image:type"u8, spo.Image?.EncodingFormat);
        WriteMetaPropContent(ref w, "og:image:alt"u8, spo.Title);

        if (spo.Image?.Width > 0 && spo.Image?.Height > 0)
        {
            WriteMetaPropContent(ref w, "og:image:width"u8, spo.Image?.Width.ToString());
            WriteMetaPropContent(ref w, "og:image:height"u8, spo.Image?.Height.ToString());
        }

        // Twitter
        if (!string.IsNullOrEmpty(req.TwitterSite))
        {
            w.Write(TwitterCard);
            WriteMetaNameUrl(ref w, "twitter:url"u8, req.Url, spo.Path);
            WriteMetaNameContent(ref w, "twitter:title"u8, spo.Title);
            WriteMetaNameContent(ref w, "twitter:description"u8, spo.Description);
            WriteMetaNameContent(ref w, "twitter:site"u8, req.TwitterSite);
            WriteMetaNameContent(ref w, "twitter:image"u8, spo.Image?.Url);
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

    // Create <link rel="icon"> tag
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void WriteIcon(
        ref Utf8BufferWriter w,
        int size,
        string? url,
        string? encodingFormat,
        ReadOnlySpan<byte> rel)
    {
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(encodingFormat))
            return;

        w.Write(LinkRelOpen);
        w.Write(rel);

        w.Write(LinkHref);
        w.Write(url);

        w.Write(LinkSizes);
        WriteInt(ref w, size);
        w.Write(X);
        WriteInt(ref w, size);

        w.Write(LinkTypeOpen);
        w.Write(encodingFormat);

        w.Write(TagClose);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void WriteInt(ref Utf8BufferWriter w, int value)
    {
        Span<byte> buffer = stackalloc byte[11];
        if (Utf8Formatter.TryFormat(value, buffer, out var written))
            w.Write(buffer[..written]);
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