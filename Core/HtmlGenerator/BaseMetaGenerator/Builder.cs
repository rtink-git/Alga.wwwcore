using System.Text;

namespace Alga.wwwcore.Core.HtmlGenerator.BaseMetaGenerator;

public class Builder
{
    public StringBuilder Do(Req req)
    {
        var sb = new StringBuilder();

        sb.Append("<meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");

        if (!string.IsNullOrEmpty(req.YandexVerificationCode)) sb.Append($$"""<meta name="yandex-verification" content="{{req.YandexVerificationCode}}">""");

        if (req.GoogleSiteVerification != null) sb.Append($$"""<meta name="google-site-verification" content="{{req.GoogleSiteVerification}}">""");

        if (req.BingSiteVerification != null) sb.Append($$"""<meta name="msvalidate.01" content="{{req.BingSiteVerification}}">""");

        if (req.PreconnectUrls?.Length > 0)
            foreach (var url in req.PreconnectUrls)
                sb.Append(PreconnectLink(url, true));

        if (req.GoogleFontsUrl is { Length: > 0 })
        {
            sb.Append(PreconnectLink("https://fonts.googleapis.com"));
            sb.Append(PreconnectLink("https://fonts.gstatic.com", true));
            sb.Append($"""<link rel="stylesheet" href="{req.GoogleFontsUrl}&font-display=swap" as="style" onload="this.rel='stylesheet'" crossorigin="anonymous">""");
        }

        sb.Append(IconLinkHtml(32, "icon"));
        sb.Append(IconLinkHtml(180, "apple-touch-icon"));

        if (req.UseMessagePack) sb.Append("<script src=\"https://cdn.jsdelivr.net/npm/@msgpack/msgpack@2.8.0/dist.es5+umd/msgpack.min.js\"></script>");

        if (req.UseTelegram) sb.Append("<script src=\"https://telegram.org/js/telegram-web-app.js\" crossorigin=\"anonymous\" defer></script>");

        if (!string.IsNullOrEmpty(req.GoogleAnalyticsCode)) sb.Append(GoogleAnalitysScript(req.GoogleAnalyticsCode));
        if (!string.IsNullOrEmpty(req.YandexMetrikaCode)) sb.Append(YandexMetrikaScripts(req.YandexMetrikaCode));

        if (!string.IsNullOrEmpty(req.ThemeColor)) sb.Append($$"""<meta name="theme-color" content="{{req.ThemeColor}}">""");

        if (!req.IsDebug) sb.Append($"<link rel=\"manifest\" href=\"/manifest.{req.Version}.json\">");
        if (!req.IsDebug) sb.Append($"<script src=\"/app.{req.Version}.js\" defer></script>");

        return sb;
    }

    // Create <link rel="preconnect">
    static string PreconnectLink(string url, bool isCrossorigin = false) => "<link rel=\"preconnect\" href=\"" + url + "\" " + (isCrossorigin ? "crossorigin" : "") + ">";

    // Create <link rel="icon"> tag
    static string IconLinkHtml(int size, string rel = "icon") => $"<link rel=\"{rel}\" href=\"/Modules/Total/content/Icon-{size}.png\" sizes=\"{size}x{size}\" type=\"image/png\">";

    // Generates Google Analytics (gtag.js) tracking script for HTML pages
    static string GoogleAnalitysScript(string googleAnalyticsCode) =>
        string.IsNullOrEmpty(googleAnalyticsCode)
            ? string.Empty
            : $"<script async src=\"https://www.googletagmanager.com/gtag/js?id={googleAnalyticsCode}\"></script>" +
              "<script>" +
              "window.dataLayer = window.dataLayer || [];" +
              "function gtag() {{ dataLayer.push(arguments); }}" +
              "gtag('js', new Date());" +
              $"gtag('config', '{googleAnalyticsCode}');" +
              "</script>";

    // Generates Yandex Metrika (counter) tracking script for HTML pages
    static string YandexMetrikaScripts(string yandexMetrikaCode) =>
        string.IsNullOrEmpty(yandexMetrikaCode)
            ? string.Empty
            : $$"""
                <script async src="https://mc.yandex.ru/metrika/tag.js"></script>
                <script type="text/javascript">
                    (function(m,e,t,r,i,k,a){
                        m[i]=m[i]||function(){(m[i].a=m[i].a||[]).push(arguments)};
                        m[i].l=1*new Date();
                        for(var j=0;j<document.scripts.length;j++){
                            if(document.scripts[j].src===r)return;
                        }
                        k=e.createElement(t),a=e.getElementsByTagName(t)[0],
                        k.async=1,k.src=r,a.parentNode.insertBefore(k,a)
                    })
                    (window,document,"script","https://mc.yandex.ru/metrika/tag.js","ym");
                    ym({{yandexMetrikaCode}},"init",{clickmap:true,trackLinks:true,accurateTrackBounce:true});
                </script>
                <noscript>
                    <div>
                        <img src="https://mc.yandex.ru/watch/{{yandexMetrikaCode}}" 
                            style="position:absolute;left:-9999px;" alt="" />
                    </div>
                </noscript>
            """;
}
