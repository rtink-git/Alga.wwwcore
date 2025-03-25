using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace Alga.wwwcore;

class _UISchemes
{
    readonly ConfigM ConfigM;
    readonly ILogger? logger;
    internal _UISchemes(ConfigM configM, ILoggerFactory? loggerFactory) {
        this.ConfigM = configM;
        if(loggerFactory != null) logger = loggerFactory.CreateLogger<_UISchemes>();
    }

    internal List<M> Build() {
        var l = new List<M>();

        var f = CheckPrerequisites();

        var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var UISsPath = Path.Combine(wwwrootPath, "UISs");

        if(Directory.Exists(wwwrootPath) && Directory.Exists(UISsPath)) {
            foreach(var uis in Directory.GetDirectories(UISsPath)) {
                var name = new DirectoryInfo(uis).Name;
                var files = Directory.GetFiles(uis);

                string? nameJs = null;
                string? nameCss = null;
                foreach(var i in files) {
                    var info = new FileInfo(i);
                    var ext = info.Extension;
                    if(ext == ".js" && !info.Name.EndsWith(".min.js")) nameJs = info.Name.Substring(0, info.Name.Length - 3);
                    else if(ext == ".css" && !info.Name.EndsWith(".min.css")) nameCss = info.Name.Substring(0, info.Name.Length - 4);
                }

                if(!string.IsNullOrEmpty(nameJs)) {
                    var schemeUrl = Path.Combine(uis, "scheme.json");
                    SchemeJsonM? schemeJsonM = null; 
                    if(File.Exists(schemeUrl)) {
                        string jsonString = File.ReadAllText(schemeUrl);
                        schemeJsonM = JsonSerializer.Deserialize<SchemeJsonM>(jsonString);
                    
                    }

                    string? title = null;
                    string? description = null;
                    string? robot = null;
                    var modulesCss = new List<Module>() { new Module("/Modules/Total/style.css", true) };
                    var modulesJs = new List<Module>() { new Module("/Modules/Total/script.js", true) };

                    if(schemeJsonM != null) {
                        title = schemeJsonM.title;
                        description = schemeJsonM.description;
                        robot = schemeJsonM.robot;

                        if(schemeJsonM.modules != null)
                            foreach(var i in schemeJsonM.modules) {
                                var path = i.path.TrimEnd('/');
                                var u = wwwrootPath + path;
                                if(Directory.Exists(u))
                                    foreach(var j in Directory.GetFiles(u)) {
                                        var info = new FileInfo(j);
                                        var ext = info.Extension;
                                        if(ext == ".js") modulesJs.Add( new Module(path + "/" + info.Name, i.toBundlerMinifier ?? false ));
                                        else if(ext == ".css") modulesCss.Add(new Module(path + "/" + info.Name, i.toBundlerMinifier ?? false));
                                    }
                                else if(File.Exists(u)) {
                                    var info = new FileInfo(u);
                                    var ext = info.Extension;
                                    if(ext == ".js") modulesJs.Add(new Module(path, i.toBundlerMinifier));
                                    else if(ext == ".css") modulesCss.Add(new Module(path, i.toBundlerMinifier));
                                }
                            }
                    }

                    var head = new StringBuilder();
                    HeadBuild(head, name, nameCss, nameJs, modulesCss, modulesJs);

                    var bodyDown = new StringBuilder();
                    BodyDownBuild(bodyDown);

                    
                    l.Add(new M(){ UISName=name, title= title, description = description, robot=robot, ModulesJs = modulesJs, ModulesCss = modulesCss, Head=head.ToString(), BodyDown=bodyDown.ToString() });
                }
            }
        }

        return l;
    }

    bool CheckPrerequisites() {
        bool f = false;

        var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        if(Directory.Exists(wwwrootPath)) {
            var UISsPath = Path.Combine(wwwrootPath, "UISs");
            if(Directory.Exists(wwwrootPath)) {
                var modulesPath = Path.Combine(wwwrootPath, "Modules");
                if(Directory.Exists(modulesPath)) {
                    var totalPath = Path.Combine(modulesPath, "Total");
                    if(Directory.Exists(totalPath)) {
                        var contentPath = Path.Combine(totalPath, "content");
                        if(Directory.Exists(contentPath)) {
                            var contentFiles = Directory.GetFiles(contentPath);

                            foreach(var i in new[] { 32, 180, 192, 512 }) {
                                var fileF = false;
                                foreach(var j in contentFiles){
                                    var info = new FileInfo(j);
                                    if(info != null) {
                                        if(info.Name == $"Icon-{i}.png") {
                                            fileF = true;
                                            break;
                                        }
                                    }
                                }
                                if(!fileF)
                                    if(this.logger != null) logger.LogWarning($"\"wwwroot/Modules/Total/content\" directory does not contain Icon-{i}.png file. It is needed for SEO to work correctly.");
                            }
                        }
                        else if(this.logger != null) logger.LogWarning("\"wwwroot/Modules/Total\" directory does not contain the \"content\" directory");
                    }
                    else if(this.logger != null) logger.LogWarning("\"wwwroot/Modules\" directory does not contain the \"Total\" directory");
                }
                else if(this.logger != null) logger.LogWarning("\"wwwroot\" directory does not contain the \"Modules\" directory");

                f = true;
            }
            else if(this.logger != null) logger.LogError("\"wwwroot\" directory does not contain the \"UISs\" directory, which is required for the library to work");
        }
        else if(this.logger != null) logger.LogCritical("Your project does not contain a public directory \"wwwroot\" in the root of your solution");

        return f;
    }

    void HeadBuild(StringBuilder head, string name, string? nameCss, string nameJs, List<Module> modulesCss, List<Module>? modulesJs) {
        var links = new StringBuilder();
        if(modulesCss != null)
            foreach(var i in modulesCss)
                if(i.toBundlerMinifier != null)
                if(this.ConfigM.IsDebug) links.Append(LinkHtml(i.path));
                else if(i.toBundlerMinifier != true) links.Append(LinkHtml(i.path));

        if(nameCss != null) links.Append(LinkHtml($"/UISs/{name}/{nameCss}" + (this.ConfigM.IsDebug ? string.Empty : ".min") + ".css"));

        var scripts = new StringBuilder();
        if(modulesJs != null)
            foreach(var i in modulesJs)
                if(this.ConfigM.IsDebug) scripts.Append(ScriptHtml(i.path));
                else if(i.toBundlerMinifier != true) scripts.Append(ScriptHtml(i.path));

        head.Append(this.ConfigM.PreconnectUrls != null ? string.Join(string.Empty, this.ConfigM.PreconnectUrls.Select(url => PreconnectLink(url))) : string.Empty)
        .Append(PageHeadGoogleFontPreconects())
        .Append(this.ConfigM.GoogleFontsUrl != null ? $"<link rel=\"stylesheet\" href=\"{this.ConfigM.GoogleFontsUrl}\" media=\"print\" onload=\"this.media='all'\"><noscript><link href=\"{this.ConfigM.GoogleFontsUrl}\" rel=\"stylesheet\"></noscript>" : null) //$"<link rel=\"preload\" href=\"{this.ConfigM.GoogleFontsUrl}\" as=\"style\" onload=\"this.rel='stylesheet'\">" : null
        .Append(links)
        .Append(PageHeadIcons())
        .Append("<link rel=\"manifest\" href=\"/manifest.json\"/>")
        .Append(PageHeadAsyncScriptsAndAnalitys())
        .Append(scripts)
        .Append(ScriptHtml($"/UISs/{name}/{nameJs}" + (this.ConfigM.IsDebug ? string.Empty : ".min") + ".js", true));
    }

    void BodyDownBuild (StringBuilder bodyDown) {
        bodyDown.Append("<script src=\"/app.js\" type=\"text/javascript\" async></script>")
        .Append(PageYandexMetrikaScripts());
    }

    string PreconnectLink(string url, bool isCrossorigin = false) => "<link rel=\"preconnect\" href=\"" + url + "\" " + ((isCrossorigin) ? "crossorigin" : "") + "/>";

    string PageHeadGoogleFontPreconects()
    {
        if (this.ConfigM.GoogleFontsUrl == null) return string.Empty;
        return $"{PreconnectLink("https://fonts.googleapis.com")}{PreconnectLink("https://fonts.gstatic.com", true)}";
    }

    string PageHeadIcons()
    {
        var html = new StringBuilder();
        foreach (var size in new[] { 32, 180 }) {
            var rel = size == 180 ? "apple-touch-icon" : "icon";
            html.Append(IconLinkHtml(size, rel));
        }
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
               $"<noscript><div><img src=\"https://mc.yandex.ru/watch/{this.ConfigM.YandexMetrikaCode}\" style=\"position:absolute; left:-9999px;\" alt=\"\"/></div></noscript>";
    }

    string LinkHtml(string url) => $"<link rel=\"stylesheet\" href=\"{url}\" type=\"text/css\"/>";
    string ScriptHtml(string url, bool isUIS=false) => "<script " + ((isUIS) ? "type=\"module\"" : "type=\"text/javascript\"") + " src=\"" + url + "\"></script>";
    string IconLinkHtml(int size, string rel = "icon") => $"<link rel=\"{rel}\" href=\"/Modules/Total/content/Icon-{size}.png\" sizes=\"{size}x{size}\" type=\"image/png\" alt=\"RT.ink Icon\">";
    record class SchemeJsonM (
        string? title = null,
        string? description = null,
        string? robot = null,
        List<Module>? modules = null
    );

    internal record Module ( string path, bool? toBundlerMinifier );
    internal enum FileTypeEnum { Js, Css }

    internal class M {
        internal required string UISName { get; set; }
        internal string? title { get; set; }
        internal string? description { get; set; }
        internal string? robot { get; set; }
        internal List<Module>? ModulesJs { get; set; }
        internal List<Module>? ModulesCss { get; set; }
        internal string? Head { get; set; }
        internal string? BodyDown { get; set; }
    }
}