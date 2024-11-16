using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Alga.wwwcore;

class _Response {
    ConfigM ConfigM { get; }
    _UISchemes.M UIScheme { get; }

    public _Response(ConfigM config, _UISchemes.M _UISchemes) { this.ConfigM = config; this.UIScheme = _UISchemes; }

    public async Task Send(HttpContext context, SeoM? seoM = null, int cacheControlInS = -1)
    {
        seoM = (seoM == null && this.UIScheme!=null && this.UIScheme.title != null) ? new SeoM(Title: this.UIScheme.title, Description: this.UIScheme.description, Robot:this.UIScheme.robot) : null;

        // Формируем HTML страницу
        var html = new StringBuilder();
        html.Append("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />")
            .Append(seoM?.MergeTags(this.ConfigM) ?? string.Empty)
            .Append(UIScheme?.Head ?? string.Empty)
            .Append("</head><body>")
            .Append(UIScheme?.BodyDown ?? string.Empty)
            .Append("</body></html>");

        // Обработка заголовков кэширования
        if (cacheControlInS == -1) cacheControlInS = ConfigM.CacheControlInSDefault;
        if (this.ConfigM.IsDebug) cacheControlInS = -1;
        if (cacheControlInS > -1) context.Response.Headers[HeaderNames.CacheControl] = $"public, max-age={cacheControlInS}";

        // Устанавливаем тип контента и отправляем ответ
        context.Response.ContentType = "text/HTML";
        await context.Response.WriteAsync(html.ToString());
    }
}

// Lines now: 45 / 104 / 155 - Before