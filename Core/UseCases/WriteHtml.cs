using System.Buffers;
using System.Text.Json;

namespace Alga.wwwcore.Core.UseCases;

public class WriteHtml
{
    private readonly IInitializer _initializer;

    public WriteHtml(IInitializer initializer) => _initializer = initializer;

    public void Do(IBufferWriter<byte> writer, string UISName, SeoPageOptions seoPageOptions, string? pageModelAsJson = null)
    {
        if (!_initializer.Pages.TryGetValue(UISName, out var pageVal)) return;

        var htmlCheckSum = JsonSerializer.Serialize(seoPageOptions).Length + (pageModelAsJson != null ? pageModelAsJson.Length : 0);

        if (pageVal.Html != null && pageVal.HtmlCheckSum == htmlCheckSum) writer.Write(pageVal.Html);
        else
        {
            var reqSeoMetaGenerator = new HtmlGenerator.SeoMetaGenerator.Req { SeoPageOptions = seoPageOptions, Url = _initializer.BaseUrl, NameShort = _initializer.AppNameShort, TwitterSite = _initializer.AppNameShort };

            var req = new HtmlGenerator.PageGenerator.Req() { IsDebug = _initializer.IsDebug, PageModelAsJson = pageModelAsJson, PageScheme = pageVal, PagesModules = _initializer.PagesModules, BaseMetaSb = _initializer.BaseMetaHeadHtml, SeoMetaReq = reqSeoMetaGenerator };

            new HtmlGenerator.PageGenerator.Builder().WriteTo(writer, req);
        }
    }
}
