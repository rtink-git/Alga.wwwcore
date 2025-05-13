using Microsoft.Extensions.Logging;
using System.Text;

namespace Alga.wwwcore;
public class _Index {
    readonly ILogger? _Logger;
    readonly Root.ConfigModel _Config;
    readonly List<Models.SchemeJsonM> _Schemes;
    readonly Dictionary<string, HashSet<string>> _Modules;

    public _Index(Root.ConfigModel config, List<Models.SchemeJsonM> schemes, Dictionary<string, HashSet<string>> modules, ILogger? logger = null) {
        _Config = config;
        _Schemes = schemes;
        _Modules = modules;
        _Logger = logger;
    }

    public async Task Build() {
        _Logger?.LogInformation("");

        var currentDirPathFull = Directory.GetCurrentDirectory();

        var wwwroot_dir = "wwwroot";
        var wwwroot_path_full = Path.Combine(currentDirPathFull, wwwroot_dir);

        foreach (var i in _Schemes) {
            // Lang

            var lang = _Config.Lang;
            var langStr = "";
            if(!string.IsNullOrEmpty(lang)) langStr = $"lang='{lang}'";

            // SEO

            var seo = new SeoM(i.title ?? "", "", i.description, i.robot);

            // Html

            var html = new StringBuilder()
            .Append($"<!DOCTYPE html><html {langStr}><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />")
            .Append(seo?.MergeTags(_Config) ?? string.Empty)
            .Append(_Config.PreconnectUrls != null ? string.Join(string.Empty, _Config.PreconnectUrls.Select(url => PreconnectLink(url, true))) : string.Empty)
            .Append(PageHeadGoogleFontPreconects())
            .Append(IconLinkHtml(32, "icon") + IconLinkHtml(180, "apple-touch-icon"))
            .Append(_Config.GoogleFontsUrl != null ? $@"<link rel=""preload"" href=""{_Config.GoogleFontsUrl}"" as=""style""><link rel=""stylesheet"" href=""{_Config.GoogleFontsUrl}"" media=""print"" onload=""this.media='all'"">" : null)
            //.Append(PageHeadAsyncScriptsAndAnalitys() + PageYandexMetrikaScripts())
            //.Append(PageYandexMetrikaScripts())
            .Append("<script src=\"https://telegram.org/js/telegram-web-app.js\"></script>");

            // Scripts & Styles

            var scst = new StringBuilder();

            if(i.modules != null)
                foreach(var j in i.modules) {
                    if(_Modules.TryGetValue(j, out var val))
                        foreach(var u in val) {
                            if(u.EndsWith(".css")) scst.Append(LinkHtml($"{u}"));
                            else if(u.EndsWith(".js")) scst.Append(ScriptHtml($"{u}"));
                        }
                }

            if(!string.IsNullOrWhiteSpace(i.style)) scst.Append(LinkHtml(i.style));
            if(!string.IsNullOrWhiteSpace(i.script)) scst.Append(ScriptImportHtml(i.script));

            var htmlFinish = "<link rel=\"manifest\" href=\"/manifest.json\"/><meta name=\"theme-color\" content=\"" + _Config.ThemeColor + "\"><script src=\"/app.js\" type=\"text/javascript\" async></script></head><body></body></html>";

            var path = wwwroot_path_full + i.script?.Replace("script.js", string.Empty);

            await File.WriteAllTextAsync(path + "index.debug.html", html.ToString() + scst.ToString() + htmlFinish.ToString());
            await File.WriteAllTextAsync(path + "index.html", html.ToString() + scst.ToString().Replace(".js", ".min.js").Replace(".css", ".min.css") + htmlFinish.ToString());
        }
    }

    string IconLinkHtml(int size, string rel = "icon") => $"<link rel=\"{rel}\" href=\"/Modules/Total/content/Icon-{size}.png\" sizes=\"{size}x{size}\" type=\"image/png\" alt=\"RT.ink Icon\">";
    string LinkHtml(string url) => $"<link rel=\"stylesheet\" href=\"{url}\" type=\"text/css\"/>";
    string PreconnectLink(string url, bool isCrossorigin = false) => "<link rel=\"preconnect\" href=\"" + url + "\" " + ((isCrossorigin) ? "crossorigin" : "") + ">";
    string PageHeadAsyncScriptsAndAnalitys() => _Config.GoogleAnalyticsCode is null ? string.Empty : $"<script async src=\"https://www.googletagmanager.com/gtag/js?id={_Config.GoogleAnalyticsCode}\"></script><script>window.dataLayer = window.dataLayer || [];function gtag(){{dataLayer.push(arguments);}}gtag('js', new Date());gtag('config', '{_Config.GoogleAnalyticsCode}');</script>";
    string PageHeadGoogleFontPreconects() => _Config.GoogleFontsUrl == null ? string.Empty : $"{PreconnectLink("https://fonts.googleapis.com")}{PreconnectLink("https://fonts.gstatic.com", true)}";
    string PageYandexMetrikaScripts() => string.IsNullOrEmpty(_Config.YandexMetrikaCode) ? string.Empty : $"<script async src=\"https://mc.yandex.ru/metrika/tag.js\"></script><script type=\"text/javascript\">(function(m,e,t,r,i,k,a){{m[i]=m[i]||function(){{(m[i].a=m[i].a||[]).push(arguments);}};m[i].l=1*new Date();for(var j=0;j<document.scripts.length;j++){{if(document.scripts[j].src===r){{return;}}}}k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)}})(window,document,\"script\",\"https://mc.yandex.ru/metrika/tag.js\",\"ym\");ym({_Config.YandexMetrikaCode},\"init\",{{clickmap:true,trackLinks:true,accurateTrackBounce:true}});</script><noscript><div><img src=\"https://mc.yandex.ru/watch/{_Config.YandexMetrikaCode}\" style=\"position:absolute;left:-9999px;\" alt=\"\"/></div></noscript>";
    string ScriptHtml(string url, bool isUIS=false) => "<script " + ((isUIS) ? "type=\"module\"" : "type=\"text/javascript\"") + " src=\"" + url + "\" " + ((isUIS) ? "crossorigin=\"anonymous\"" : "") + "></script>";
    string ScriptImportHtml(string path) {
        var pathSplit = path.Trim('/').Split('/');
        var name = pathSplit[pathSplit.Length - 2];
        return $"<script type=\"module\">import {{ {name}UIS }} from '{path}'; {name}UIS();</script>";
    }
}