using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.IO;

namespace Alga.wwwcore;

public abstract class UIsBase
{
    bool _IsDebug { get; }
    ConfigModel Config;

    string Wwwroot_Components;
    public string Wwwroot_ExternalComponents;
    string Wwwroot_UIRs;

    public UIsBase(bool isDebug, ConfigModel config) {
        this._IsDebug = isDebug;
        this.Config = config;

        this.Wwwroot_Components = "/Components";
        this.Wwwroot_ExternalComponents = "/ExternalComponents";
        this.Wwwroot_UIRs = "/UIRs";

        if(this._IsDebug) ManifestJsonGenerate();
    }

    protected List<UrlModel> CompleateComponents(MethodBase? pageMethodBase, UrlModel[] l, List<UrlModel> lsub) { var ll = new List<UrlModel>(); ll.AddRange(l); ll.AddRange(lsub); ll.Add(new UrlModel(pageMethodBase, FilesTypes.JsAndCss, ComponentTypes.UI)); return ll; }

    // -- Response to the client

    public async Task Response(HttpContext context, List<UrlModel> heads, string? seoTags = null, string? headsSub = null, int cacheControlInS = -1)
    {
        if(!this._IsDebug) {
            var pageName = "";
            foreach(var i in heads)
                if(i.componentType == ComponentTypes.UI && i.methodBase != null) { pageName = i.methodBase.Name; break; }
            await ResponseRelease(context, pageName, seoTags, headsSub, cacheControlInS);
        }
        else await ResponseDebug(context, heads, seoTags, headsSub);
    }

    async Task ResponseDebug(HttpContext context, List<UrlModel> heads, string? seoTags, string? headsSub)
    {
        var hd = "";
        foreach (var i in heads)
        {
            var url = ((i.componentType == ComponentTypes.UIComponent) ? this.Wwwroot_Components : this.Wwwroot_UIRs) + "/" + ((i.methodBase != null) ? i.methodBase.Name : "");;

            if (i.filesType == FilesTypes.JsAndCss || i.filesType == FilesTypes.CssOnly) hd += LinkHtml(url + "/style.css");
            if (i.filesType == FilesTypes.JsAndCss || i.filesType == FilesTypes.JsOnly) hd += ScriptHtml(url + "/script.js", i.componentType);
        }

        context.Response.ContentType = "text/HTML";
        await ResponseBase(context, seoTags, headsSub + hd);
    }

    async Task ResponseRelease(HttpContext context, string pageName, string? seoTags, string? headsSub = null, int cacheControlInS = -1)
    {
        if(cacheControlInS == -1) cacheControlInS = Config.UICacheControlInSDefault;
        if(cacheControlInS > -1) context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=" + cacheControlInS;
        context.Response.ContentType = "text/HTML";

        var hd = LinkHtml(this.Wwwroot_UIRs + "/" + pageName + "/style.min.css") + ScriptHtml(this.Wwwroot_UIRs + "/" + pageName + "/script.min.js", ComponentTypes.UI);
        await ResponseBase(context, seoTags, headsSub + hd);
    }

    async Task ResponseBase(HttpContext context, string? seoTags, string heads) => await context.Response.WriteAsync("<!DOCTYPE html><html><head>" + PageHeadMeta() + seoTags + PageHeadPreconect() + PageHeadLinks() + PageHeadIconsAndManifest() + PageHeadAsyncScriptsAndAnalitys() + PageHeadScripts() + heads + "</head><body>" + PageBodyDownScripts() + "</body></html>");

    // --------------------

    string LinkHtml(string url) => "<link rel=\"stylesheet\" href=\"" + url + "\" type=\"text/css\" />";
    string ScriptHtml(string url, ComponentTypes componentType = ComponentTypes.UIComponent) => "<script " + ((componentType == ComponentTypes.UI) ? "type=\"module\"" : "") + " src=\"" + url + "\" type=\"text/javascript\"></script>";

    // --------------------

    string PageHeadMeta() {
        var html = "";
        html += "<meta charset=\"utf-8\">";
        html += "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />";
        return html;
    }

    string PageHeadPreconect() {
        var html = "";
        if(Config.PreconnectUrls != null)
            foreach(var i in Config.PreconnectUrls) html += PreconnectLink(i);
        if(Config.GoogleFontsUrl != null) {
            html += PreconnectLink("https://fonts.googleapis.com");
            html += PreconnectLink("https://fonts.gstatic.com", true);
        }
        return html;
    }

    string PageHeadLinks() {
        var html = "";
        if(Config.GoogleFontsUrl != null) html += "<link href=\"" + Config.GoogleFontsUrl + "\" rel=\"stylesheet\">";
        return html;
    }

    string PageHeadIconsAndManifest() {
        var html = "";
        html += IconLinkHtml(32);
        html += IconLinkHtml(48);
        html += IconLinkHtml(64);
        html += IconLinkHtml(70);
        html += IconLinkHtml(120, "apple-touch-icon");
        html += IconLinkHtml(150, "msapplication-TileImage");
        html += IconLinkHtml(152, "apple-touch-icon");
        html += IconLinkHtml(167, "apple-touch-icon");
        html += IconLinkHtml(180, "apple-touch-icon");
        html += IconLinkHtml(192);
        html += IconLinkHtml(310, "msapplication-TileImage");
        html += IconLinkHtml(512);
        
        // PWA manifest: https://web.dev/articles/add-manifest
        html += "<link rel=\"manifest\" href=\"/manifest.json\" />";
        return html;
    }

    string PageHeadAsyncScriptsAndAnalitys() {
        var html = "";
        
        // Google analytics

        if(Config.GoogleAnalyticsCode != null) {
            html += "<script async src=\"https://www.googletagmanager.com/gtag/js?id=" + Config.GoogleAnalyticsCode + "\"></script>";
            // Google analytics - script
            html += "<script>window.dataLayer = window.dataLayer || []; function gtag(){dataLayer.push(arguments);} gtag('js', new Date()); gtag('config', '" + Config.GoogleAnalyticsCode + "');</script>";
        }
        return html;
    }

    string PageHeadScripts()
    {
        var html = "";
        return html;
    }

    string PageBodyDownScripts() {
        var html = "";
        html += "<script src=\"/app.js\" type=\"text/javascript\" async></script>";
        
        // Yandex metrika

        if(Config.YandexMetrikaCode != null) {
            html += "<script async src=\"https://mc.yandex.ru/metrika/tag.js\"></script>";
            html += "<script type=\"text/javascript\">(function(m,e,t,r,i,k,a){m[i]=m[i]||function(){(m[i].a=m[i].a||[]).push(arguments)};m[i].l=1*new Date();for (var j = 0; j < document.scripts.length; j++) {if (document.scripts[j].src === r) { return; }} k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)})(window, document, \"script\", \"https://mc.yandex.ru/metrika/tag.js\", \"ym\");ym(" + Config.YandexMetrikaCode + ", \"init\", { clickmap:true,trackLinks:true,accurateTrackBounce:true });</script>";
            if(Config.YandexMetrikaCode != null)
                html += "<noscript><div><img src=\"https://mc.yandex.ru/watch/" + Config.YandexMetrikaCode + "\" style=\"position:absolute; left:-9999px;\" alt=\"\" /></div></noscript>";
        }
        return html;      
    }

    string PreconnectLink(string url, bool isCrossorigin = false) => "<link rel=\"preconnect\" href=\"" + url + "\" " + ((isCrossorigin) ? "crossorigin" : "") + " />";

    string IconLinkHtml(int size, string rel = "icon") => "<link rel=\"" + rel + "\" href=\"/Components/Total/content/Icon-" + size + ".png\" sizes=\"" + size + "x" + size + "\" type=\"image/png\">";
    

    // -- BundlerMinifier
    // -- nuget: https://www.nuget.org/packages/BundlerMinifier.Core
    // -- nuget: https://www.nuget.org/packages/BuildBundlerMinifier

    //public void BundleconfigJsonRebuild(List<List<UrlModel>> l)
    protected void BundleconfigJsonRebuild(Type type)
    {
        var l = new List<List<UrlModel>>();
        
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach(var i in methods)
            if(i.IsPublic && i.ReturnType == typeof(List<UrlModel>)) {
                object? r = i.Invoke(this,null);
                if(r != null) l.Add((List<UrlModel>)r);
            }

        var ll = new List<BundleModel>();

        foreach (var i in l)
        {
            var pageName = "";
            var scripts = new List<string>();
            var styles = new List<string>();

            foreach (var j in i)
                if(j.methodBase != null) {
                    if (j.componentType == ComponentTypes.UI)
                        pageName = j.methodBase.Name;

                    var url = "wwwroot" + ((j.componentType == ComponentTypes.UIComponent) ? this.Wwwroot_Components : this.Wwwroot_UIRs) + "/" + j.methodBase.Name;

                    if (j.filesType == FilesTypes.JsAndCss || j.filesType == FilesTypes.JsOnly) scripts.Add(url + "/script.js");
                    if (j.filesType == FilesTypes.JsAndCss || j.filesType == FilesTypes.CssOnly) styles.Add(url + "/style.css");
                }

            if(pageName.Length > 0) {
                var minifyModel = new MinifyModel() { enabled = true, renameLocals = true };
                ll.Add(new BundleModel() { outputFileName = "wwwroot" + this.Wwwroot_UIRs + "/" + pageName + "/script.min.js", inputFiles = scripts, minify = minifyModel });
                ll.Add(new BundleModel() { outputFileName = "wwwroot" + this.Wwwroot_UIRs + "/" + pageName + "/style.min.css", inputFiles = styles, minify = minifyModel });
            }
        }

        using FileStream createStream = File.Create(Environment.CurrentDirectory + "/bundleconfig.json");
        JsonSerializer.SerializeAsync(createStream, ll);
    }

    class BundleModel
    {
        public string? outputFileName { get; set; }
        public List<string>? inputFiles { get; set; }
        public MinifyModel? minify { get; set; }
    }

    class MinifyModel {
        public bool enabled { get; set; }
        public bool renameLocals { get; set; }
    }

    // -- Models

    public record UrlModel(MethodBase? methodBase, FilesTypes filesType = FilesTypes.JsAndCss, ComponentTypes componentType = ComponentTypes.UIComponent);
    public enum FilesTypes { JsAndCss, JsOnly, CssOnly }
    public enum ComponentTypes { UIComponent, UI, ExternalComponent }

    // --

    void ManifestJsonGenerate() {
        var manifestModel = new ManifestModel() {
            name = Config.Name,
            short_name = Config.NameShort,
            description = Config.Description,
            background_color = Config.BackgroundColor,
            theme_color = Config.ThemeColor,
            icons = new List<Icon>() {
                new Icon() { src = "/Components/Total/content/Icon-192", sizes="192x192", type="image/png" },
                new Icon() { src = "/Components/Total/content/Icon-512", sizes="512x512", type="image/png" }
            }
        };

        var url = Environment.CurrentDirectory + "/wwwroot/manifest.json";
        if(!File.Exists(url)) {
            using FileStream createStream = File.Create(Environment.CurrentDirectory + "/wwwroot/manifest.json");
            JsonSerializer.SerializeAsync(createStream, manifestModel);
        }
    }

    class ManifestModel {
        public string id { get; set;} = "/";
        public string scope { get; set; } = "/";
        public string start_url { get; set;} = "/";
        public string display  {get; set;} = "standalone";
        public string name { get; set; } = "";
        public string short_name { get; set;} = "";
        public string description { get; set; } = "";
        public string background_color { get; set; } = "#FFFFFF";
        public string theme_color { get; set; } = "#FFFFFF";
        public List<Icon>? icons { get; set; } = null;
    }

    public class Icon {
        public string? src { get; set; } = null;
        public string? type { get; set; } = null;
        public string? sizes { get; set; } = null;
    }

    public string SEO(string title, string? description = null, string? robot = null, string? urlCanonical = null, string? imageUrl = null)
    {
        var html = "";

        //-- base settings

        if (robot != null) html += "<meta name=\"robots\" content=\"" + robot + "\">";
        if (description != null) html += "<meta name=\"description\" content=\"" + description + "\">";
        if (urlCanonical != null) html += "<meta rel=\"canonical\" href=\"" + Config.Url + urlCanonical + "\">";

        // -- og

        if (!string.IsNullOrEmpty(Config.Name) && !string.IsNullOrEmpty(title))
        {
            html += "<meta property=\"og:site_name\" content=\"" + Config.Name + "\">";
            html += "<meta property=\"og:type\" content=\"article\">";
            html += "<meta property=\"og:url\" content=\"" + Config.Url + "\">";
            html += "<meta property=\"og:title\" content=\"" + title + "\">";
            if (!string.IsNullOrEmpty(description)) { html += "<meta property=\"og:description\" content=\"" + description + "\">"; }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                html += "<meta property=\"og:image\" content=\"" + Config.Url + imageUrl + "\">";
                html += "<meta property=\"og:image:alt\" content=\"" + title + "\">";
            }
        }

        // -- twitter

        if (!string.IsNullOrEmpty(Config.TwitterSite) && !string.IsNullOrEmpty(title))
        {
            html += "<meta name=\"twitter:site\" content=\"" + Config.TwitterSite + "\">";
            html += "<meta name=\"twitter:card\" content=\"summary_large_image\">";
            html += "<meta name=\"twitter:description\" content=\"summary_large_image\">";
            html += "<meta name=\"twitter:title\" content=\"" + title + "\">";
            if (!string.IsNullOrEmpty(description)) { html += "<meta name=\"twitter:description\" content=\"" + description + "\">"; }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                html += "<meta property=\"name:twitter:image\" content=\"" + imageUrl + "\">";
                html += "<meta property=\"name:twitter:image:alt\" content=\"" + title + "\">";
            }
        }

        html += "<title>" + title + "</title>";

        return html;
    }
}
