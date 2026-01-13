using System.Collections.Frozen;
using System.Text;

namespace Alga.wwwcore.Core;

public class Initializer : IInitializer
{
    public bool IsDebug { get; init; }
    public FrozenDictionary<string, SchemesGenerator.PageModel> Pages { get; set; } = FrozenDictionary<string, SchemesGenerator.PageModel>.Empty; // Frozen page schemes collection. All directories in wwwroot that contain scheme.json
    public FrozenDictionary<string, HashSet<string>> PagesModules { get; set; } = FrozenDictionary<string, HashSet<string>>.Empty; // Frozen pages modules schemes collection. Frozen collection of components that use the pages.
    public StringBuilder BaseMetaHeadHtml { get; set; }
    public string BaseUrl { get; init; }
    public string AppNameShort { get; init; }
    public string? AppTwitterSite { get; init; }

    readonly Operations.GetClientOptions.Res _clientOptions;

    public Initializer(Operations.GetClientOptions.Res clientOptions, bool isDebug)
    {
        _clientOptions = clientOptions;

        BaseUrl = clientOptions.BaseUrl;
        AppNameShort = clientOptions.NameShort;
        AppTwitterSite = clientOptions.TwitterSite;
        IsDebug = isDebug;
        BaseMetaHeadHtml = new StringBuilder();
    }

    public void Do()
    {
        var version = DateTime.UtcNow.ToString("yyyyMMddHHmm");
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        if (!Directory.Exists(directoryPath)) throw new DirectoryNotFoundException($"Required wwwroot directory not found at: {directoryPath}");


        var reqBaseMetaGenerator = new HtmlGenerator.BaseMetaGenerator.Req() { IsDebug = IsDebug, Version = version, BingSiteVerification = _clientOptions.BingSiteVerification, GoogleAnalyticsCode = _clientOptions.GoogleAnalyticsCode, GoogleFontsUrl = _clientOptions.GoogleFontsUrl, GoogleSiteVerification = _clientOptions.GoogleSiteVerification, PreconnectUrls = _clientOptions.PreconnectUrls, ThemeColor = _clientOptions.ThemeColor, UseMessagePack = _clientOptions.UseMessagePack, UseTelegram = _clientOptions.UseTelegram, YandexMetrikaCode = _clientOptions.YandexMetrikaCode, YandexVerificationCode = _clientOptions.YandexVerificationCode };
        var baseMetaGeneratorDone = new HtmlGenerator.BaseMetaGenerator.Builder().Do(reqBaseMetaGenerator);

        BaseMetaHeadHtml = baseMetaGeneratorDone;

        var reqSchemesGenerator = new SchemesGenerator.Req() { IsDebug = IsDebug, Version = version, DirectoryPath = directoryPath };
        var schemesGeneratorDone = new SchemesGenerator.Builder().Do(reqSchemesGenerator);

        Pages = schemesGeneratorDone.Pages.ToFrozenDictionary();
        PagesModules = schemesGeneratorDone.PagesModules.ToFrozenDictionary();

        if (!IsDebug)
        {
            var reqManifestJson = new FileGenerators.ManifestJson.Req() { Version = version, DirectoryPath = directoryPath, Name = _clientOptions.Name, NameShort = _clientOptions.NameShort, Description = _clientOptions.Description, BackgroundColor = _clientOptions.BackgroundColor, ThemeColor = _clientOptions.ThemeColor };
            new FileGenerators.ManifestJson.Builder().Do(reqManifestJson);

            var reqAppJs = new FileGenerators.AppJs.Req() { Version = version, DirectoryPath = directoryPath, BackgroundColor = _clientOptions.BackgroundColor };
            new FileGenerators.AppJs.Builder().Do(reqAppJs);

            var reqServiceworkerJs = new FileGenerators.ServiceworkerJs.Req() { Version = version, DirectoryPath = directoryPath, Schemes = Pages, OfflinePageUrl = _clientOptions.OfflinePageUrlPath, CacheUrls = _clientOptions.CacheUrls };
            new FileGenerators.ServiceworkerJs.Builder().Do(reqServiceworkerJs);

            foreach (var i in Pages)
            {
                var reqPageJsFile = new HtmlGenerator.PageJsFile.Req() { Version = version, Key = i.Key, DirectoryPath = directoryPath, PageModel = i.Value, PageModules = PagesModules };
                new HtmlGenerator.PageJsFile.Builder().Create(reqPageJsFile);

                var reqPageCssFile = new HtmlGenerator.PageCssFile.Req() { Version = version, Key = i.Key, DirectoryPath = directoryPath, PageModel = i.Value, PageModules = PagesModules };
                new HtmlGenerator.PageCssFile.Builder().Create(reqPageCssFile);
            }
        }
    }
}