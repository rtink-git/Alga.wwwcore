using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Alga.wwwcore;

public abstract class UIsBase
{
    ConfigModel Config;
    public UIsBase(ConfigModel config) =>  this.Config = config;

    protected List<UrlModel> CompleateComponents(MethodBase? pageMethodBase, UrlModel[] l, List<UrlModel> lsub) { var ll = new List<UrlModel>(); ll.AddRange(l); ll.AddRange(lsub); ll.Add(new UrlModel(pageMethodBase, FilesTypes.JsAndCss, ComponentTypes.UI)); return ll; }

    // -- Response to the client

    public async Task Response(HttpContext context, List<UrlModel> heads, string? seoTags = null, string? headsSub = null, int cacheControlInS = -1)
    {
        if(!Config.IsDebug) {
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
            var url = ((i.componentType == ComponentTypes.UIComponent) ? Config.WebRootUrlPart_Activities_Components : Config.WebRootUrlPart_Activities_UIRs) + "/" + ((i.methodBase != null) ? i.methodBase.Name : "");;

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

        var hd = LinkHtml(Config.WebRootUrlPart_Activities_UIRs + "/" + pageName + "/style.min.css") + ScriptHtml(Config.WebRootUrlPart_Activities_UIRs + "/" + pageName + "/script.min.js", ComponentTypes.UI);
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
        if(Config.Icon180Url != null) html += "<link rel=\"apple-touch-icon\" href=\"" + Config.Icon180Url + "/logo180.png\">";
        if(Config.Icon32Url != null) html += "<link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"" + Config.Icon32Url + "\">";
        if(Config.Icon192Url != null) html += "<link rel=\"icon\" type=\"image/png\" sizes=\"192x192\" href=\"" + Config.Icon192Url + "\">";
        if(Config.Icon512Url != null) html += "<link rel=\"icon\" type=\"image/png\" sizes=\"512x512\" href=\"" + Config.Icon512Url + "\">";
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

    // -- BundlerMinifier
    // -- nuget: https://www.nuget.org/packages/BundlerMinifier.Core
    // -- nuget: https://www.nuget.org/packages/BuildBundlerMinifier

    //public void BundleconfigJsonRebuild(List<List<UrlModel>> l)
    public void BundleconfigJsonRebuild(Type type)
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

                    var url = "wwwroot" + ((j.componentType == ComponentTypes.UIComponent) ? Config.WebRootUrlPart_Activities_Components : Config.WebRootUrlPart_Activities_UIRs) + "/" + j.methodBase.Name;

                    if (j.filesType == FilesTypes.JsAndCss || j.filesType == FilesTypes.JsOnly) scripts.Add(url + "/script.js");
                    if (j.filesType == FilesTypes.JsAndCss || j.filesType == FilesTypes.CssOnly) styles.Add(url + "/style.css");
                }

            if(pageName.Length > 0) {
                var minifyModel = new MinifyModel() { enabled = true, renameLocals = true };
                ll.Add(new BundleModel() { outputFileName = "wwwroot" + Config.WebRootUrlPart_Activities_UIRs + "/" + pageName + "/script.min.js", inputFiles = scripts, minify = minifyModel });
                ll.Add(new BundleModel() { outputFileName = "wwwroot" + Config.WebRootUrlPart_Activities_UIRs + "/" + pageName + "/style.min.css", inputFiles = styles, minify = minifyModel });
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

    public string SEO(string title, string? description = null, string? robot = null, string? urlCanonical = null, string? imageUrl = null)
    {
        var html = "";

        //-- base settings

        if (robot != null) html += "<meta name=\"robots\" content=\"" + robot + "\">";
        if (description != null) html += "<meta name=\"description\" content=\"" + description + "\">";
        if (urlCanonical != null) html += "<meta rel=\"canonical\" href=\"" + Config.url + urlCanonical + "\">";

        // -- og

        if (!string.IsNullOrEmpty(Config.name) && !string.IsNullOrEmpty(title))
        {
            html += "<meta property=\"og:site_name\" content=\"" + Config.name + "\">";
            html += "<meta property=\"og:type\" content=\"article\">";
            html += "<meta property=\"og:url\" content=\"" + Config.url + "\">";
            html += "<meta property=\"og:title\" content=\"" + title + "\">";
            if (!string.IsNullOrEmpty(description)) { html += "<meta property=\"og:description\" content=\"" + description + "\">"; }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                html += "<meta property=\"og:image\" content=\"" + Config.url + imageUrl + "\">";
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

    // -- Model

    public record ConfigModel(
        /// <summary>
        /// Building status
        /// Example: true (is debug) / false (is release)
        /// </summary>
        bool IsDebug = true,
        /// <summary>
        /// Project name
        /// Example: Myproject
        /// </summary>
        string? name = null,
        /// <summary>
        /// Project url
        /// Example: (Alga.wwwcore.Config.IsDebug) ? "https://localhost:1234" : "https://example.com"
        /// </summary>
        string? url = null,
        string? WebRootUrlPart_Activities_Components =  "/Components",
        string? WebRootUrlPart_Activities_ExternalComponents = "/ExternalComponents",
        string? WebRootUrlPart_Activities_UIRs = "/UIRs",
        int UICacheControlInSDefault = -1,
        /// <summary>
        /// Project Icon 32x32 px.
        /// Recomended url (is default): "wwwroot/Components/Total/content/Icon-32.png"
        /// </summary>
        string? Icon32Url = "/Components/Total/content/Icon-32.png",
        /// <summary>
        /// Project Icon 180x180 px.
        /// Recomended url (is default): "wwwroot/Components/Total/content/Icon-180.png"
        /// </summary>
        string? Icon180Url = "/Components/Total/content/Icon-180.png",
        /// <summary>
        /// Project Icon 192x192 px.
        /// Recomended url (is default): "wwwroot/Components/Total/content/Icon-192.png"
        /// </summary>
        string? Icon192Url = "/Components/Total/content/Icon-192.png",
        /// <summary>
        /// Project Icon 512x512 px.
        /// Recomended url (is default): "wwwroot/Components/Total/content/Icon-512.png"
        /// </summary>
        string? Icon512Url = "/Components/Total/content/Icon-512.png",
        /// <summary>
        /// Preconnect urls
        /// Example: new List<string>() { api.example.com, api1.example.com, api2.example.com }
        /// </summary>
        List<string>? PreconnectUrls = null,
        /// <summary>
        /// Google Fonts 
        /// https://fonts.google.com/
        /// </summary>
        string? GoogleFontsUrl = null,
        /// <summary>
        /// Google Analytics Code
        /// Example: G-9DDDCCTTXX
        /// </summary>
        string? GoogleAnalyticsCode = null,
        /// <summary>
        /// Yandex Metrika Code
        /// Example: 99991111
        /// </summary>
        string? YandexMetrikaCode = null,
        /// <summary>
        /// Twitter account name
        /// Example: @ElonMusk
        /// </summary>
        string? TwitterSite = null
    );
}
