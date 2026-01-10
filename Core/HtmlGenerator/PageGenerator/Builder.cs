using System.Buffers;
using System.Text;

namespace Alga.wwwcore.Core.HtmlGenerator.PageGenerator;

class Builder
{
    public void WriteTo(IBufferWriter<byte> writer, Req req)
    {
        var sb = new StringBuilder();

        sb.Append("<!DOCTYPE html>");
        sb.Append("<html>");
        sb.Append("<head>");
        sb.Append(req.BaseMetaSb.ToString());

        if (req.SeoMetaReq != null) new SeoMetaGenerator.Builder(sb).Do(req.SeoMetaReq);

        if (req.IsDebug && req.PagesModules != null && req.PageScheme.modules != null) // && pageModulesVal is not null && pageVal.modules is not null
            foreach (var j in req.PageScheme.modules)
                if (req.PagesModules.TryGetValue(j, out var val))
                    foreach (var u in val)
                        if (u.EndsWith(".css", StringComparison.OrdinalIgnoreCase)) sb.Append(LinkHtml(u));
                        else if (u.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) sb.Append(ScriptHtml(u));

        if (!string.IsNullOrWhiteSpace(req.PageScheme.style)) sb.Append(LinkHtml(req.PageScheme.style));

        if (!string.IsNullOrWhiteSpace(req.PageScheme.script)) sb.Append(ScriptImportHtml(req.PageScheme.script));

        if (!string.IsNullOrEmpty(req.PageModelAsJson)) sb.Append($"<script id=\"page-model\" type=\"application/json\">{req.PageModelAsJson}</script>");

        sb.Append("</head>");
        sb.Append("<body>");
        sb.Append("</body>");
        sb.Append("</html>");

        foreach (ReadOnlyMemory<char> chunk in sb.GetChunks())
        {
            // Max для worst‑case (4 байта на символ); обычно будет < actualSpan.Length
            int maxBytes = Encoding.UTF8.GetMaxByteCount(chunk.Length);
            Span<byte> dest = writer.GetSpan(maxBytes);

            int written = Encoding.UTF8.GetBytes(chunk.Span, dest);
            writer.Advance(written);
        }
    }

    // Create CSS <link> tag with preload hack.
    static string LinkHtml(string url) => $"<link rel=\"stylesheet\" href=\"{url}\" as=\"style\" />"; // onload=\"this.rel='stylesheet'\"

    // Create <script> tag with defer
    static string ScriptHtml(string url) => $"<script src=\"{url}\" defer></script>";

    // Generate inline ES‑Module import wrapper
    static string ScriptImportHtml(string path)
    {
        var name = path.Trim('/').Split('/')[^2];
        return $"<script type=\"module\">import {{ {name}UIS }} from '{path}'; {name}UIS();</script>";
    }
}