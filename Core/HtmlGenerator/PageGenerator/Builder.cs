using System.IO.Pipelines;
using System.Text;
using System.Runtime.CompilerServices;

namespace Alga.wwwcore.Core.HtmlGenerator.PageGenerator;

class Builder
{
    static readonly Encoding Utf8 = Encoding.UTF8;

    public void WriteTo(PipeWriter writer, Req req)
    {
        WriteString("<!DOCTYPE html><html><head>");
        WriteString(req.BaseMetaSb.ToString());
        if (req.SeoMetaReq != null) new SeoMetaGenerator.Builder().Write(req.SeoMetaReq, writer);

        if (req.IsDebug && req.PagesModules != null && req.PageScheme.modules != null)
            foreach (var moduleName in req.PageScheme.modules)
                if (req.PagesModules.TryGetValue(moduleName, out var files))
                    foreach (var file in files)
                    {
                        if (file.EndsWith(".css", StringComparison.OrdinalIgnoreCase)) WriteLink(file);
                        else if (file.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) WriteScript(file);
                    }

        if (!string.IsNullOrWhiteSpace(req.PageScheme.style)) WriteLink(req.PageScheme.style);
        if (!string.IsNullOrWhiteSpace(req.PageScheme.script)) WriteScriptImport(req.PageScheme.script);

        if (!string.IsNullOrEmpty(req.PageModelAsJson))
        {
            WriteString("<script id=\"page-model\" type=\"application/json\">");
            WriteString(req.PageModelAsJson);
            WriteString("</script>");
        }

        WriteString("</head><body></body></html>");

        // Create CSS <link> tag with preload hack.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteLink(string url)
        {
            WriteString("<link rel=\"stylesheet\" href=\"");
            WriteString(url);
            WriteString("\" as=\"style\" />");
        }

        // Create <script> tag with defer
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteScript(string url)
        {
            WriteString("<script src=\"");
            WriteString(url);
            WriteString("\" defer></script>");
        }

        void WriteScriptImport(string path)
        {
            // name = path.Trim('/').Split('/')[^2];
            // Реализация без аллокаций

            ReadOnlySpan<char> span = path.AsSpan().Trim('/');

            int lastSlash = span.LastIndexOf('/');
            if (lastSlash <= 0) return;

            int prevSlash = span[..lastSlash].LastIndexOf('/');
            if (prevSlash < 0) return;

            ReadOnlySpan<char> name = span.Slice(
                prevSlash + 1,
                lastSlash - prevSlash - 1
            );

            WriteString("<script type=\"module\">import { ");
            WriteSpan(name);
            WriteString("UIS } from '");
            WriteString(path);
            WriteString("'; ");
            WriteSpan(name);
            WriteString("UIS();</script>");
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