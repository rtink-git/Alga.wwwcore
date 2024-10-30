using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Alga.wwwcore;

public class ActivitiesBase
{
    bool _IsDebug { get; }
    List<Url> _Urls { get; }

    protected string? Wwwroot_Activities_Components;
    protected string? Wwwroot_Activities_ExternalComponents;
    protected string? Wwwroot_Activities_UIRs;

    public ActivitiesBase(bool IsDebug, List<Url> urls) {
        this._Urls = urls;

        // -- File Syste: Check and adding default directories in the wwwroot

        string? webRootPath = null;
        foreach(var i in this._Urls)
        if(i.urlType == UrlTypes.WebRoot) webRootPath = i.url;
        if(webRootPath != null) {
            var ActivitiesURP = "/Activities";
            var Wwwroot_Activities = webRootPath + ActivitiesURP;
            if (!Directory.Exists(Wwwroot_Activities)) Directory.CreateDirectory(Wwwroot_Activities);
            this.Wwwroot_Activities_Components = ActivitiesURP + "/Components";
            if (!Directory.Exists(Wwwroot_Activities_Components)) Directory.CreateDirectory(Wwwroot_Activities + Wwwroot_Activities_Components);
            this.Wwwroot_Activities_ExternalComponents = ActivitiesURP + "/ExternalComponents";
            if (!Directory.Exists(Wwwroot_Activities_ExternalComponents)) Directory.CreateDirectory(Wwwroot_Activities + Wwwroot_Activities_ExternalComponents);
            this.Wwwroot_Activities_UIRs = ActivitiesURP + "/UIRs";
            if (!Directory.Exists(Wwwroot_Activities_UIRs)) Directory.CreateDirectory(Wwwroot_Activities + Wwwroot_Activities_UIRs);
        }
    }

    // -- Response to the client

    public async Task Response(HttpContext context, List<UrlModel> heads, string? seoTags = null, string? headsSub = null, int cacheControlInS = -1)
    {
        if(!this._IsDebug) {
            var pageName = "";
            foreach(var i in heads)
                if(i.componentType == ComponentTypes.Page && i.methodBase != null) { pageName = i.methodBase.Name; break; }
            await ResponseRelease(context, pageName, seoTags, headsSub, cacheControlInS);
        }
        else await ResponseDebug(context, heads, seoTags, headsSub);
    }

    async Task ResponseDebug(HttpContext context, List<UrlModel> heads, string? seoTags, string? headsSub)
    {
        var hd = "";
        foreach (var i in heads)
        {
            var url = ((i.componentType == ComponentTypes.PageComponent) ? this.Wwwroot_Activities_Components : this.Wwwroot_Activities_UIRs) + "/" + ((i.methodBase != null) ? i.methodBase.Name : "");;

            if (i.filesType == FilesTypes.JsAndCss || i.filesType == FilesTypes.CssOnly) hd += LinkHtml(url + "/style.css");
            if (i.filesType == FilesTypes.JsAndCss || i.filesType == FilesTypes.JsOnly) hd += ScriptHtml(url + "/script.js", i.componentType);
        }

        context.Response.ContentType = "text/HTML";
        await ResponseBase(context, seoTags, headsSub + hd);
    }

    async Task ResponseRelease(HttpContext context, string pageName, string? seoTags, string? headsSub = null, int cacheControlInS = -1)
    {
        if (cacheControlInS > -1) context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=" + cacheControlInS;
        context.Response.ContentType = "text/HTML";

        var hd = LinkHtml(this.Wwwroot_Activities_UIRs + "/" + pageName + "/style.min.css") + ScriptHtml(this.Wwwroot_Activities_UIRs + "/" + pageName + "/script.min.js", ComponentTypes.Page);
        await ResponseBase(context, seoTags, headsSub + hd);
    }

    async Task ResponseBase(HttpContext context, string? seoTags, string heads) => await context.Response.WriteAsync("<!DOCTYPE html><html><head>" + PageHeadMeta() + seoTags + PageHeadPreconect() + PageHeadLinks() + PageHeadIconsAndManifest() + PageHeadAsyncScriptsAndAnalitys() + PageHeadScripts() + heads + "</head><body>" + PageBodyDownScripts() + "</body></html>");

    public List<UrlModel> CompleateComponents(MethodBase? pageMethodBase, UrlModel[] l, List<UrlModel> lsub) { var ll = new List<UrlModel>(); ll.AddRange(l); ll.AddRange(lsub); ll.Add(new UrlModel(pageMethodBase, FilesTypes.JsAndCss, ComponentTypes.Page)); return ll; }

    // --------------------

    string LinkHtml(string url) => "<link rel=\"stylesheet\" href=\"" + url + "\" type=\"text/css\" />";
    string ScriptHtml(string url, ComponentTypes componentType = ComponentTypes.PageComponent) => "<script " + ((componentType == ComponentTypes.Page) ? "type=\"module\"" : "") + " src=\"" + url + "\" type=\"text/javascript\"></script>";

    // --------------------

    string PageHeadMeta() {
        var html = "";
        html += "<meta charset=\"utf-8\">";
        html += "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />";
        return html;
    }

    string PageHeadPreconect() {
        var html = "";
        foreach(var i in this._Urls) if(i.urlType == UrlTypes.Preconnect) html += PreconnectLink(i.url);
        var hasGFonts = false;
        foreach(var i in this._Urls) if(i.urlType == UrlTypes.GoogleFont) { hasGFonts = true; break;}
        if(hasGFonts) {
            html += PreconnectLink("https://fonts.googleapis.com");
            html += PreconnectLink("https://fonts.gstatic.com", true);
        }
        return html;
    }

    string PageHeadLinks() {
        var html = "";
        foreach(var i in this._Urls) if(i.urlType == UrlTypes.GoogleFont) html += "<link href=\"" + i.url + "\" rel=\"stylesheet\">";
        return html;
    }

    string PageHeadIconsAndManifest() {
        var html = "";
        var url = this.Wwwroot_Activities_Components + "/HeadHtmlBox/content";
        html += "<link rel=\"apple-touch-icon\" href=\"" + url + "/logo180.png\">";
        html += IconLink(32);
        html += IconLink(144);
        html += IconLink(192);
        html += IconLink(512);
        // PWA manifest: https://web.dev/articles/add-manifest
        html += "<link rel=\"manifest\" href=\"/manifest.json\" />";
        return html;
    }

    string PageHeadAsyncScriptsAndAnalitys() {
        var html = "";
        
        // Google analytics
        
        string? googleAnalyticsCode = null;
        foreach(var i in this._Urls) if(i.urlType == UrlTypes.GoogleAnalyticsCode) googleAnalyticsCode = i.url;
            if(googleAnalyticsCode != null) {
            html += "<script async src=\"https://www.googletagmanager.com/gtag/js?id=" + googleAnalyticsCode + "\"></script>";
            // Google analytics - script
            html += "<script>window.dataLayer = window.dataLayer || []; function gtag(){dataLayer.push(arguments);} gtag('js', new Date()); gtag('config', '" + googleAnalyticsCode + "');</script>";
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

        string? yandexMetrikaCode = null;
        foreach(var i in this._Urls) if(i.urlType == UrlTypes.YandexMetrikaCode) yandexMetrikaCode = i.url;
        if(yandexMetrikaCode != null) {
            html += "<script async src=\"https://mc.yandex.ru/metrika/tag.js\"></script>";
            html += "<script type=\"text/javascript\">(function(m,e,t,r,i,k,a){m[i]=m[i]||function(){(m[i].a=m[i].a||[]).push(arguments)};m[i].l=1*new Date();for (var j = 0; j < document.scripts.length; j++) {if (document.scripts[j].src === r) { return; }} k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)})(window, document, \"script\", \"https://mc.yandex.ru/metrika/tag.js\", \"ym\");ym(" + yandexMetrikaCode + ", \"init\", { clickmap:true,trackLinks:true,accurateTrackBounce:true });</script>";
            html += "<noscript><div><img src=\"https://mc.yandex.ru/watch/" + yandexMetrikaCode + "\" style=\"position:absolute; left:-9999px;\" alt=\"\" /></div></noscript>";
        }
        return html;      
    }

    string IconLink(int size) => "<link rel=\"icon\" type=\"image/png\" rel=\"noopener\" target=\"_blank\" sizes=\"" + size + "x" + size + "\" href=\"" + this.Wwwroot_Activities_Components + "/HeadHtmlBox/content/logo512.png\">";
    string PreconnectLink(string url, bool isCrossorigin = false) => "<link rel=\"preconnect\" href=\"" + url + "\" " + ((isCrossorigin) ? "crossorigin" : "") + " />";

    // -- Models

    public enum FilesTypes { JsAndCss, JsOnly, CssOnly }
    public enum ComponentTypes { PageComponent, Page, ExternalComponent }
    public enum UrlTypes { WebRoot, GoogleFont, Preconnect, GoogleAnalyticsCode, YandexMetrikaCode }
    public record UrlModel(MethodBase? methodBase, FilesTypes filesType = FilesTypes.JsAndCss, ComponentTypes componentType = ComponentTypes.PageComponent);
    public record Url (UrlTypes urlType, string url);

    // -- BundlerMinifier
    // -- nuget: https://www.nuget.org/packages/BundlerMinifier.Core
    // -- nuget: https://www.nuget.org/packages/BuildBundlerMinifier

    public void BundleconfigJsonRebuild(List<List<UrlModel>> l)
    {
        var ll = new List<BundleModel>();

        foreach (var i in l)
        {
            var pageName = "";
            var scripts = new List<string>();
            var styles = new List<string>();

            foreach (var j in i)
                if(j.methodBase != null) {
                    if (j.componentType == ComponentTypes.Page)
                        pageName = j.methodBase.Name;

                    var url = "wwwroot" + ((j.componentType == ComponentTypes.PageComponent) ? this.Wwwroot_Activities_Components : this.Wwwroot_Activities_UIRs) + "/" + j.methodBase.Name;

                    if (j.filesType == FilesTypes.JsAndCss || j.filesType == FilesTypes.JsOnly) scripts.Add(url + "/script.js");
                    if (j.filesType == FilesTypes.JsAndCss || j.filesType == FilesTypes.CssOnly) styles.Add(url + "/style.css");
                }

            if(pageName.Length > 0) {
                var minifyModel = new MinifyModel() { enabled = true, renameLocals = true };
                ll.Add(new BundleModel() { outputFileName = "wwwroot" + this.Wwwroot_Activities_UIRs + "/" + pageName + "/script.min.js", inputFiles = scripts, minify = minifyModel });
                ll.Add(new BundleModel() { outputFileName = "wwwroot" + this.Wwwroot_Activities_UIRs + "/" + pageName + "/style.min.css", inputFiles = styles, minify = minifyModel });
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
}
