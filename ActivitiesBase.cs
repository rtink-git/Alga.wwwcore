using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Alga.wwwcore;

public class ActivitiesBase
{
    public ActivitiesBase() {

        // -- File Syste: Check and adding default directories in the wwwroot

        if(Config.WebRootUrl != null) {
            var Wwwroot_Activities = Config.WebRootUrl + Config.WebRootUrlPart_Activities;
            if (!Directory.Exists(Wwwroot_Activities)) Directory.CreateDirectory(Wwwroot_Activities);
            var Wwwroot_Activities_Components = Config.WebRootUrl + Config.WebRootUrlPart_Activities_Components;
            if (!Directory.Exists(Wwwroot_Activities_Components)) Directory.CreateDirectory(Wwwroot_Activities_Components);
            var Wwwroot_Activities_ExternalComponents = Config.WebRootUrl + Config.WebRootUrlPart_Activities_ExternalComponents;
            if (!Directory.Exists(Wwwroot_Activities_ExternalComponents)) Directory.CreateDirectory(Wwwroot_Activities_ExternalComponents);
            var Wwwroot_Activities_UIRs = Config.WebRootUrl + Config.WebRootUrlPart_Activities_UIRs;
            if (!Directory.Exists(Wwwroot_Activities_UIRs)) Directory.CreateDirectory(Wwwroot_Activities_UIRs);
        }
    }

    // -- Response to the client

    public async Task Response(HttpContext context, List<UrlModel> heads, string? seoTags = null, string? headsSub = null, int cacheControlInS = -1)
    {
        if(!Config.IsDebug) {
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
            var url = ((i.componentType == ComponentTypes.PageComponent) ? Config.WebRootUrlPart_Activities_Components : Config.WebRootUrlPart_Activities_UIRs) + "/" + ((i.methodBase != null) ? i.methodBase.Name : "");;

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

        var hd = LinkHtml(Config.WebRootUrlPart_Activities_UIRs + "/" + pageName + "/style.min.css") + ScriptHtml(Config.WebRootUrlPart_Activities_UIRs + "/" + pageName + "/script.min.js", ComponentTypes.Page);
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
            html += "<noscript><div><img src=\"https://mc.yandex.ru/watch/" + Config.YandexMetrikaCode + "\" style=\"position:absolute; left:-9999px;\" alt=\"\" /></div></noscript>";
        }
        return html;      
    }

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

                    var url = "wwwroot" + ((j.componentType == ComponentTypes.PageComponent) ? Config.WebRootUrlPart_Activities_Components : Config.WebRootUrlPart_Activities_UIRs) + "/" + j.methodBase.Name;

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
}
