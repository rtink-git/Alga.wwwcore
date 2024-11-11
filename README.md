# Alga.wwwcore

The purpose of this package is to simplify the development of web applications in ASP.NET Core

- You define the scheme of user interfaces and their components
- Automatically detects the head for correct operation of the application
- Automatically generated "bundleconfig.json" which bundles all javascript & css UI components into one file and compresses it
- Automatically generated "manifest.json" which is necessary for PWA to work correctly
- Tools to add SEO instructions for every user interface


### How does this work. Step by step

1. Create ASP.NET Core project or open an existing project

2. Add [Alga.wwwcore](https://www.nuget.org/packages/Alga.wwwcore) nuget package

`> dotnet add package Alga.wwwcore --version 1.0.14` (check last version)

3. Add [BundlerMinifier](https://www.nuget.org/packages/BuildBundlerMinifier) nuget package

`> dotnet add package BuildBundlerMinifier --version 3.2.449`

4. Open "wwwroot" directory (if not exist - create it)

- Create "Components" dirictory (wwwroot/Components)
- - Create "Total" dirictory (wwwroot/Components/Total) - is main component
- - - Create "content" dirictory (wwwroot/Components/Total/content) - this is where images and other files will be stored
- - - - Add "Icon-32.png" (32x32px) (wwwroot/Components/Total/content/Icon-32.png) - will be used as an Favicon icon
- - - - Add "Icon-48.png" (48x48px) (wwwroot/Components/Total/content/Icon-38.png) - will be used as an Favicon icon
- - - - Add "Icon-64.png" (64x64px) (wwwroot/Components/Total/content/Icon-64.png) - will be used as an Favicon icon
- - - - Add "Icon-70.png" (70x70px) (wwwroot/Components/Total/content/Icon-70.png) - will be used as an Microsoft Tiles icon
- - - - Add "Icon-120.png" (120x120px) (wwwroot/Components/Total/content/Icon-120.png) - will be used as an Apple Touch icon
- - - - Add "Icon-150.png" (150x150px) (wwwroot/Components/Total/content/Icon-150.png) - will be used as an Microsoft Tiles icon
- - - - Add "Icon-152.png" (152x152px) (wwwroot/Components/Total/content/Icon-152.png) - will be used as an Apple Touch icon
- - - - Add "Icon-167.png" (167x167px) (wwwroot/Components/Total/content/Icon-167.png) - will be used as an Apple Touch icon
- - - - Add "Icon-180.png" (180x180px) (wwwroot/Components/Total/content/Icon-180.png) - will be used as an Apple Touch icon
- - - - Add "Icon-192.png" (192x192px) (wwwroot/Components/Total/content/Icon-192.png) - will be used as an Android and PWA icon
- - - - Add "Icon-310.png" (310x310px) (wwwroot/Components/Total/content/Icon-310.png) - will be used as an Microsoft Tiles icon
- - - - Add "Icon-512.png" (512x512px) (wwwroot/Components/Total/content/Icon-512.png) - will be used as an Android and PWA icon
- Create "ExternalComponents" dirictory (wwwroot/ExternalComponents)
- Create "UIRs" dirictory (wwwroot/UIRs)


5. Create "wwwroot.cs" file (/wwwroot.cs) - A scheme that follows the directory (with files) structure of "wwwroot" directory.
The class "wwwroot" must inherit the abstract class "SchemeB" from Alga.wwwcore

Code example: 

```
using System.Reflection;
using Alga.wwwcore;

public class wwwroot: SchemeB
{
    ConfigM Config;
    public wwwroot(ConfigM config): base(config) => this.Config = config;

    // -- User Interface Screens (Pages)
    // -- Path: wwwroot/UISs - derectory. Here are the located of your project UISs (Pages)
    // -- Path: wwwroot/UISs/{Index} - UIS (Page) directory. Here are the located files that are necessary to display the UIS (Page)
    // -- Path: wwwroot/UISs/{Index}/script.js - Javascript file with executable code that builds the UIS (Page) (runs modules and other UIS's (Page's) code) and displays it on the device screen. Recommended file name: script.js.
    // -- Path: wwwroot/UISs/{Index}/style.css - UIS (Page) styles file. Recommended file name: style.css.  If the page consists of modules with their own styles, it is not necessary to add this file
    // -- Code example: public ScreenM Index() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), ListGHtmlBox(), MoreButtonUI()], [], this.Config );
    // -- Code description:
    // -- "Index" - method name that matches the directory located on the path: wwwroot/UISs/Index.
    // -- MethodBase.GetCurrentMethod() - Getting the object "Method" with which we get the name of this method (Index)
    // -- DirСontainFilesEnum.JsAndCss - Specify what type of files are in the directory
    // -- [ HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), ListGHtmlBox(), MoreButtonUI() ] - Add "ModuleM" standard modules, that run on this page (executable script.js code)
    // -- [ MomentjsCom() ] - Add "ModuleExtM" standard modules, which can be used regardless executable script.js code
    // -- this.Config - Project configuration params

    public ScreenM bookmarks() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ ApiRtInk_MSAP(), HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), ListGHtmlBox(), MoreButtonUI()], [], this.Config );
    public ScreenM Error() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ ApiRtInk_MSAP(), HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox() ], [], this.Config);
    public ScreenM i() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ ApiRtInk_MSAP(), HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), ListGHtmlBox(), MoreButtonUI() ], [ MomentjsCom() ], this.Config);
    public ScreenM Locs() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ ApiRtInk_MSAP(), HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), LocationsHtmlBox_20231116(), TitleDescriptionAHtmlBox() ], [], this.Config);
    public ScreenM Settings() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ ApiRtInk_MSAP(), HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), ButtonDeleteHtmlBox(), SelectFormHtmlBox(), UserIconHtmlBox(), InputTextFormHtmlBox(), TextareaFormHtmlBox(), TitleDescriptionAHtmlBox() ], [], this.Config );
    public ScreenM u() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ ApiRtInk_MSAP(), HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), ListGHtmlBox(), TitleDescriptionUserHtmlBox(), MoreButtonUI(), UserIconHtmlBox() ], [], this.Config);
    public ScreenM users() => new SchemeB.ScreenM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsAndCss, [ ApiRtInk_MSAP(), HeadHtmlBox(), HeaderTextUI(), FooterInfHtmlBox(), UsersHtmlBox(), UserIconHtmlBox(), MoreButtonUI() ], [], this.Config);

    // -- UIS Modules of standart "ModuleM"
    // -- Path: wwwroot/Modules - derectory. Here are the located modules that can be used by any of the UIS (Page)
    // -- Path: wwwroot/Modules/{FooterInfHtmlBox} - Module directory.
    // -- Path: wwwroot/UISs/{FooterInfHtmlBox}/script.js - Javascript file with Module class. Recommended file name: script.js. The module may not support the script.css
    // -- Path: wwwroot/UISs/{FooterInfHtmlBox}/style.css - Module styles file. Recommended file name: style.css. The module may not support the style.css)
    // -- Code example: ModuleM FooterInfHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    // -- "FooterInfHtmlBox" - method name that matches the directory located on the path: wwwroot/Modules/Index.
    // -- MethodBase.GetCurrentMethod() - Getting the object "Method" with which we get the name of this method (FooterInfHtmlBox)
    // -- DirСontainFilesEnum.JsAndCss - Specify what type of files are in the directory

    ModuleM ApiRtInk_MSAP() => new ModuleM(MethodBase.GetCurrentMethod(), DirСontainFilesEnum.JsOnly);
    ModuleM ButtonDeleteHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM FooterInfHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM HeaderTextUI() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM HeadHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM InputTextFormHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM ListGHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM LocationsHtmlBox_20231116() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM MoreButtonUI() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM SelectFormHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM TextareaFormHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM TitleDescriptionAHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM TitleDescriptionUserHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM UserIconHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());
    ModuleM UsersHtmlBox() => new ModuleM(MethodBase.GetCurrentMethod());

    // -- UIS External Modules
    // -- UIS Modules of standart "ModuleExtM". All other modules or files that must be executed by the UIS (Page)
    // -- Code example: ModuleExtM MomentjsCom() => new ModuleExtM(MethodBase.GetCurrentMethod(), "/ExternalComponents/MomentjsCom/moment-timezone.min.js");
    // -- MethodBase.GetCurrentMethod() - Getting the object "Method" with which we get the name of this method (MomentjsCom)
    // -- "/ExternalComponents/MomentjsCom/moment-timezone.min.js" - Path to file

    ModuleExtM MomentjsCom() => new ModuleExtM(MethodBase.GetCurrentMethod(), "/ExternalComponents/MomentjsCom/moment-timezone.min.js");
}

```

6. Program.cs & "Alga.wwwcore" nuget package

Code example:

```
var isDebug = true;

if (!app.Environment.IsDevelopment())
{
    isDebug = false;
    ...
}

...

var w = new wwwroot(new ConfigM(
    IsDebug: isDebug,
    Url: (isDebug) ? "https://localhost:6543" : "https://example.com",
    Name: "My Store project",
    NameShort: "My Store",
    Description: "About my store, short information",
    CacheControlInSDefault: 3*60*60,
    PreconnectUrls: [ RtInk.Constants.url_api ],
    GoogleFontsUrl: "https://fonts.googleapis.com/css2?family=Audiowide&family=Montserrat:wght@500;600;700&family=Nunito:wght@500;700&Mulish:wght@500&display=swap",
    GoogleAnalyticsCode: "G-111EEEE",
    YandexMetrikaCode: "12342221"
));

app.MapGet("/", async (context) => { await w.i().SendAsync(context, await i_seo(context)); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/a/{urlShort}", async (HttpContext context, IHttpClientFactory httpClientFactory) => { await w.i().SendAsync(context, await i_seo(context, httpClientFactory)); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/bookmarks", async (context) => { await w.bookmarks().SendAsync(context, new SeoM("Bookmarks", "", "noindex")); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/error", async (context) => { await w.Error().SendAsync(context, new SeoM("Error - RT", "Page was not found", "noindex")); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/i/{search}", async (HttpContext context, IHttpClientFactory httpClientFactory) => { await w.i().SendAsync(context, await i_seo(context, httpClientFactory)); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/locs", async (context) => { await w.Locs().SendAsync(context, new SeoM("Countries", "Choose your country. Subscribe to them and read the news.")); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/settings", async (context) => { await w.Settings().SendAsync(context, new SeoM("Settings", "User settings, followers and followings", "noindex")); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/u/{login-search}", async (HttpContext context, IHttpClientFactory httpClientFactory) => { await w.u().SendAsync(context, await u_seo(context, httpClientFactory)); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/users", async (context) => { await w.users().SendAsync(context, new SeoM("Users", "")); }).CacheOutput(ThreeHOutputCachePolicy);
```