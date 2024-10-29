using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Alga.wwwcore;

public class ActivitiesBase
{
    protected string Wwwroot_Activities_Components;
    protected string Wwwroot_Activities_ExternalComponents;
    protected string Wwwroot_Activities_UIRs;

    public ActivitiesBase() {
        this.Wwwroot_Activities_Components = "/Activities/Components";
        this.Wwwroot_Activities_ExternalComponents = "/Activities/ExternalComponents";
        this.Wwwroot_Activities_UIRs = "/Activities/UIRs";
    }

    // -- Response to the client

    public async Task Response(HttpContext context, List<UrlModel> heads, string? seoTags = null, string? headsSub = null, int cacheControlInS = -1)
    {
#if !DEBUG
        var pageName = "";
        foreach(var i in heads) 
            if(i.componentType == ComponentTypes.Page) { pageName = i.methodBase.Name; break; }
        await ResponseRelease(context, pageName, seoTags, headsSub, cacheControlInS);
#endif
#if DEBUG
        await ResponseDebug(context, heads, seoTags, headsSub);
#endif
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
        //html += "<link rel=\"preconnect\" href=\"" + RtInk.Constants.url_api + "\" />";
        html += "<link rel=\"preconnect\" href=\"https://fonts.googleapis.com\" />";
        html += "<link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin />";
        return html;
    }

    string PageHeadLinks() {
        var html = "";
        html += "<link href=\"https://fonts.googleapis.com/css2?family=Audiowide&family=Montserrat:wght@500;600;700&family=Nunito:wght@500;700&Mulish:wght@500&display=swap\" rel=\"stylesheet\">";
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
        html += "<script async src=\"https://www.googletagmanager.com/gtag/js?id=G-3JWL2CTE3M\"></script>";
        // Google analytics - script
        html += "<script>window.dataLayer = window.dataLayer || []; function gtag(){dataLayer.push(arguments);} gtag('js', new Date()); gtag('config', 'G-3JWL2CTE3M');</script>";
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
        html += "<script async src=\"https://mc.yandex.ru/metrika/tag.js\"></script>";
        html += "<script type=\"text/javascript\">(function(m,e,t,r,i,k,a){m[i]=m[i]||function(){(m[i].a=m[i].a||[]).push(arguments)};m[i].l=1*new Date();for (var j = 0; j < document.scripts.length; j++) {if (document.scripts[j].src === r) { return; }} k=e.createElement(t),a=e.getElementsByTagName(t)[0],k.async=1,k.src=r,a.parentNode.insertBefore(k,a)})(window, document, \"script\", \"https://mc.yandex.ru/metrika/tag.js\", \"ym\");ym(97622081, \"init\", { clickmap:true,trackLinks:true,accurateTrackBounce:true });</script>";
        html += "<noscript><div><img src=\"https://mc.yandex.ru/watch/97622081\" style=\"position:absolute; left:-9999px;\" alt=\"\" /></div></noscript>";
        return html;      
    }

    string IconLink(int size) => "<link rel=\"icon\" type=\"image/png\" rel=\"noopener\" target=\"_blank\" sizes=\"" + size + "x" + size + "\" href=\"" + this.Wwwroot_Activities_Components + "/HeadHtmlBox/content/logo512.png\">";

    // -- Models

    public enum FilesTypes { JsAndCss, JsOnly, CssOnly }
    public enum ComponentTypes { PageComponent, Page, ExternalComponent }
    public record UrlModel(MethodBase? methodBase, FilesTypes filesType = FilesTypes.JsAndCss, ComponentTypes componentType = ComponentTypes.PageComponent);

    // -- BundlerMinifier
    // -- nuget: https://www.nuget.org/packages/BundlerMinifier.Core

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
