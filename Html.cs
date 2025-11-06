using System.Buffers;
using System.Collections.Frozen;
using System.Text;
using Alga.wwwcore.Models;

namespace Alga.wwwcore;

// Lightweight HTML generator that streams markup directly to an IBufferWriter.
class Html
{
    readonly Config _config;
    readonly string _googleFontsLink;
    readonly string _iconLink32;
    readonly string _iconLinkApple;
    readonly string _themeColorMeta;
    readonly string _preconnectFontsCom;
    readonly string _preconnectFontsGstatic;
    readonly string _googleAnalitysScript;
    readonly string _googleSiteVerification;
    readonly string _yandexMetrikaScripts;
    readonly string _yandexVerificationCode;
    readonly string _bingSiteVerification;
    readonly string _msgPackScript;
    readonly string _telegramScript;

    public Html(Config config)
    {
        _config = config;
        _googleFontsLink = $"""<link rel="stylesheet" href="{_config.GoogleFontsUrl}&font-display=swap" as="style" onload="this.rel='stylesheet'" crossorigin="anonymous">""";
        _iconLink32 = IconLinkHtml(32, "icon");
        _iconLinkApple = IconLinkHtml(180, "apple-touch-icon");
        _themeColorMeta = $$"""<meta name="theme-color" content="{{_config.ThemeColor}}">""";
        if (!string.IsNullOrEmpty(_config.YandexVerificationCode))
            _yandexVerificationCode = $$"""<meta name="yandex-verification" content="{{_config.YandexVerificationCode}}">""";
        if(!string.IsNullOrEmpty(_config.GoogleSiteVerification))
            _googleSiteVerification = $$"""<meta name="google-site-verification" content="{{_config.GoogleSiteVerification}}">""";
        if(!string.IsNullOrEmpty(_config.BingSiteVerification))
            _bingSiteVerification = $$"""<meta name="msvalidate.01" content="{{_config.BingSiteVerification}}">""";

        _preconnectFontsCom = PreconnectLink("https://fonts.googleapis.com");
        _preconnectFontsGstatic = PreconnectLink("https://fonts.gstatic.com", true);
        if (!string.IsNullOrEmpty(_config.GoogleAnalyticsCode))
            _googleAnalitysScript = GoogleAnalitysScript(_config.GoogleAnalyticsCode);
        if (!string.IsNullOrEmpty(_config.YandexMetrikaCode))
            _yandexMetrikaScripts = YandexMetrikaScripts(_config.YandexMetrikaCode);
        if (_config.UseMessagePack)
            _msgPackScript = "<script src=\"https://cdn.jsdelivr.net/npm/@msgpack/msgpack@2.8.0/dist.es5+umd/msgpack.min.js\"></script>";
        if (_config.UseTelegram)
            _telegramScript = "<script src=\"https://telegram.org/js/telegram-web-app.js\" crossorigin=\"anonymous\" defer></script>";
    }

    const string _documentTag = "<!DOCTYPE html>";
    const string _finishTags = "<link rel=\"manifest\" href=\"/manifest.json\"><script src=\"/app.js\" defer></script></head><body></body></html>";
    const string _startTags = "<head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">";

    // Streams a fully‑assembled HTML page to the provided <paramref name="writer"
    public void WriteTo(IBufferWriter<byte> writer, Models.SchemeJsonM pageVal, FrozenDictionary<string, HashSet<string>>? pageModulesVal = null, Models.Seo? seoM = null, string? pageModelAsJSON = null)
    {
        var sw = new StringBuilder();
        sw.Append(_documentTag);
        sw.Append($"<html lang=\"{_config.Lang}\">");
        sw.Append(_startTags);

        if(seoM != null) new Seo(sw, _config, seoM).MergeTags();

        if(_yandexVerificationCode != null) sw.Append(_yandexVerificationCode);
        if(_googleSiteVerification != null) sw.Append(_googleSiteVerification);
        if(_bingSiteVerification != null) sw.Append(_bingSiteVerification);

        if (_config.PreconnectUrls?.Count > 0)
            foreach (var url in _config.PreconnectUrls)
                sw.Append(PreconnectLink(url, true));

        if (_config.GoogleFontsUrl is { Length: > 0 })
        {
            sw.Append(_preconnectFontsCom);
            sw.Append(_preconnectFontsGstatic);
            sw.Append(_googleFontsLink);
        }

        if(seoM != null && !string.IsNullOrEmpty(seoM.ImageUrl)) sw.Append($"<link rel=\"preload\" as=\"image\" href=\"{seoM.ImageUrl}\">");

        sw.Append(_iconLink32);
        sw.Append(_iconLinkApple);

        var sbw = new StringBuilder();

        if (_config.IsDebug && pageModulesVal is not null && pageVal.modules is not null)
            foreach (var j in pageVal.modules)
                if (pageModulesVal.TryGetValue(j, out var val))
                    foreach (var u in val)
                        if (u.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                        {
                            sw.Append(LinkHtml(u));
                        }
                        else if (u.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                        {
                            //slw.Append(PreloadLinkAsScript(u));
                            sbw.Append(ScriptHtml(u));
                        }

        if (!string.IsNullOrWhiteSpace(pageVal.style)) sw.Append(LinkHtml(pageVal.style));

        if (!string.IsNullOrEmpty(pageModelAsJSON)) sw.Append($"<script id=\"page-model\" type=\"application/json\">{pageModelAsJSON}</script>");
        
        if (!string.IsNullOrWhiteSpace(pageVal.script))
        {
            //slw.Append(PreloadLinkAsScript(pageVal.script));
            sbw.Append(ScriptImportHtml(pageVal.script));
        }

        if (_msgPackScript != null) sw.Append(_msgPackScript);
        
        sw.Append(sbw);

        if (_telegramScript != null) sw.Append(_telegramScript);
        if (_googleAnalitysScript != null) sw.Append(_googleAnalitysScript);
        if(_yandexMetrikaScripts != null) sw.Append(_yandexMetrikaScripts);

        sw.Append(_themeColorMeta);
        sw.Append(_finishTags);

        foreach (ReadOnlyMemory<char> chunk in sw.GetChunks())
        {
            // Max для worst‑case (4 байта на символ); обычно будет < actualSpan.Length
            int maxBytes = Encoding.UTF8.GetMaxByteCount(chunk.Length);
            Span<byte> dest = writer.GetSpan(maxBytes);

            int written = Encoding.UTF8.GetBytes(chunk.Span, dest);
            writer.Advance(written);
        }
    }

    // Returns the rendered HTML as a ReadOnlyMemory<byte>
    public ReadOnlyMemory<byte> GetBytes(
        SchemeJsonM pageVal,
        FrozenDictionary<string, HashSet<string>>? pageModulesVal = null,
        Models.Seo? seo = null)
    {
        var writer = new ArrayBufferWriter<byte>(); // Pre-allocate reasonable size
        WriteTo(writer, pageVal, pageModulesVal, seo);
        return writer.WrittenMemory;
    }

    // Create <link rel="icon"> tag
    static string IconLinkHtml(int size, string rel = "icon") => $"<link rel=\"{rel}\" href=\"/Modules/Total/content/Icon-{size}.png\" sizes=\"{size}x{size}\" type=\"image/png\">";

    // Create CSS <link> tag with preload hack.
    static string LinkHtml(string url) => $"<link rel=\"stylesheet\" href=\"{url}\" as=\"style\" />"; // onload=\"this.rel='stylesheet'\"

    // Create <link rel="preconnect">
    static string PreconnectLink(string url, bool isCrossorigin = false) => "<link rel=\"preconnect\" href=\"" + url + "\" " + (isCrossorigin ? "crossorigin" : "") + ">";
    
    //static string PreloadLinkAsScript(string url) => "<link rel=\"preload\" href=\"" + url + "\" as=\"script\">";

    // Generate inline ES‑Module import wrapper
    static string ScriptImportHtml(string path)
    {
        var name = path.Trim('/').Split('/')[^2];
        return $"<script type=\"module\">import {{ {name}UIS }} from '{path}'; {name}UIS();</script>";
    }

    // Create <script> tag with defer
    static string ScriptHtml(string url) => $"<script src=\"{url}\" defer></script>";

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