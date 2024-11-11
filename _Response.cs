using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Alga.wwwcore;

class _Response {
    ConfigM ConfigM { get; }

    public _Response(ConfigM config) =>this.ConfigM = config;

    public async Task Send(HttpContext context, SchemeB.ScreenM screenM, SeoM? seoM = null, int cacheControlInS = -1)
    {
        var (links, scripts) = GenerateLinksAndScripts(screenM);

        // Формируем HTML страницу
        var html = new StringBuilder();
        html.Append("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />")
            .Append(seoM?.MergeTags(this.ConfigM) ?? string.Empty)
            .Append(this.ConfigM.PreconnectUrls != null ? string.Join(string.Empty, this.ConfigM.PreconnectUrls.Select(url => PreconnectLink(url))) : string.Empty)
            .Append(PageHeadGoogleFontPreconects())
            .Append(this.ConfigM.GoogleFontsUrl != null ? $"<link href=\"{this.ConfigM.GoogleFontsUrl}\" rel=\"stylesheet\">" : null)
            .Append(links)
            .Append(PageHeadIcons())
            .Append("<link rel=\"manifest\" href=\"/manifest.json\" />")
            .Append(PageHeadAsyncScriptsAndAnalitys())
            .Append(scripts)
            .Append(screenM.Method != null ? ScriptHtml($"/UISs/{screenM.Method.Name}/script" + ((this.ConfigM.IsDebug) ? null : ".min") + ".js", true) : string.Empty)
            .Append("</head><body>")
            .Append("<script src=\"/app.js\" type=\"text/javascript\" async></script>")
            .Append(PageYandexMetrikaScripts())
            .Append("</body></html>");

        // Обработка заголовков кэширования
        if (cacheControlInS == -1) cacheControlInS = ConfigM.CacheControlInSDefault;
        if (this.ConfigM.IsDebug) cacheControlInS = -1;
        if (cacheControlInS > -1) context.Response.Headers[HeaderNames.CacheControl] = $"public, max-age={cacheControlInS}";

        // Устанавливаем тип контента и отправляем ответ
        context.Response.ContentType = "text/HTML";
        await context.Response.WriteAsync(html.ToString());
    }

    // --------------------

    string PageHeadGoogleFontPreconects()
    {
        if (this.ConfigM.GoogleFontsUrl == null) return string.Empty;
        return $"{PreconnectLink("https://fonts.googleapis.com")}{PreconnectLink("https://fonts.gstatic.com", true)}";
    }

    string PageHeadIcons()
    {
        var html = new StringBuilder();
        foreach (var size in new[] { 32, 48, 64, 70, 120, 150, 152, 167, 180, 192, 310, 512 }) html.Append(IconLinkHtml(size));
        return html.ToString();
    }

    string PageHeadAsyncScriptsAndAnalitys()
    {
        if (this.ConfigM.GoogleAnalyticsCode == null) return string.Empty;
        return $"<script async src=\"https://www.googletagmanager.com/gtag/js?id={this.ConfigM.GoogleAnalyticsCode}\"></script>" +
               $"<script>window.dataLayer = window.dataLayer || []; function gtag(){{dataLayer.push(arguments);}} gtag('js', new Date()); gtag('config', '{this.ConfigM.GoogleAnalyticsCode}');</script>";
    }

    string PageYandexMetrikaScripts()
    {
        if (string.IsNullOrEmpty(this.ConfigM.YandexMetrikaCode)) return string.Empty;

        return "<script async src=\"https://mc.yandex.ru/metrika/tag.js\"></script>" +
               $"<script type=\"text/javascript\">(function(m,e,t,r,i,k,a){{m[i]=m[i]||function(){{(m[i].a=m[i].a||[]).push(arguments);}};m[i].l=1*new Date();for (var j = 0; j < document.scripts.length; j++) {{if (document.scripts[j].src === r) {{ return; }}}} k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)}})(window, document, \"script\", \"https://mc.yandex.ru/metrika/tag.js\", \"ym\");ym({this.ConfigM.YandexMetrikaCode}, \"init\", {{ clickmap:true, trackLinks:true, accurateTrackBounce:true }});</script>" +
               $"<noscript><div><img src=\"https://mc.yandex.ru/watch/{this.ConfigM.YandexMetrikaCode}\" style=\"position:absolute; left:-9999px;\" alt=\"\" /></div></noscript>";
    }

    // --------------------

    (string links, string scripts) GenerateLinksAndScripts(SchemeB.ScreenM screenM) {
        var links = new StringBuilder();
        var scripts = new StringBuilder();

        // Добавляем ссылки и скрипты из screenM.ModuleExts
        screenM.ModuleExts?.ForEach(i => {
            if (i.FileType == SchemeB.FileTypeEnum.Css) links.Append(LinkHtml(i.Url));
            else if (i.FileType == SchemeB.FileTypeEnum.Js) scripts.Append(ScriptHtml(i.Url));
        });

        // Добавляем модули с собственными стилями и скриптами
        if (this.ConfigM.IsDebug && screenM.ModuleMs != null)
            foreach (var i in screenM.ModuleMs)
                if (i.Method != null) {
                    if (i.DirСontains == SchemeB.DirСontainFilesEnum.JsAndCss || i.DirСontains == SchemeB.DirСontainFilesEnum.CssOnly) links.Append(LinkHtml($"/Modules/{i.Method.Name}/style.css"));
                    if (i.DirСontains == SchemeB.DirСontainFilesEnum.JsAndCss || i.DirСontains == SchemeB.DirСontainFilesEnum.JsOnly) scripts.Append(ScriptHtml($"/Modules/{i.Method.Name}/script.js"));
                }

        if(screenM.Method != null) links.Append(LinkHtml($"/UISs/{screenM.Method.Name}/style" + (this.ConfigM.IsDebug ? string.Empty : ".min") + ".css"));

        return (links.ToString(), scripts.ToString());
    }

    string LinkHtml(string url) => $"<link rel=\"stylesheet\" href=\"{url}\" type=\"text/css\" />";
    string ScriptHtml(string url, bool isUIS=false) => "<script " + ((isUIS) ? "type=\"module\"" : "") + " src=\"" + url + "\" type=\"text/javascript\"></script>";
    string PreconnectLink(string url, bool isCrossorigin = false) => "<link rel=\"preconnect\" href=\"" + url + "\" " + ((isCrossorigin) ? "crossorigin" : "") + " />";
    string IconLinkHtml(int size, string rel = "icon") => $"<link rel=\"{rel}\" href=\"/Modules/Total/content/Icon-{size}.png\" sizes=\"{size}x{size}\" type=\"image/png\">";
}

// Lines now: 104 / 155 - Before