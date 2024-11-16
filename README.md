# Alga.wwwcore

The Alga.wwwcore nuget package makes it easy to develop web applications in ASP.NET Core.

- Less UI logic in asp.net
- Automatically generate a `<head>` for the correct operation of the web application
- The "bindleconfig.json" file is automatically generated, which defines the rules for bindling and minifying javascript & css files for each page (UI Screen)
- Automatically generated "manifest.json" which is necessary for PWA to work correctly
- Tool to add SEO instructions for each page (user interface screen) and the ability to override them





### How does this work. Step by step

1. Create ASP.NET Core project or open an existing project

2. Add [Alga.wwwcore](https://www.nuget.org/packages/Alga.wwwcore) nuget package

`> dotnet add package Alga.wwwcore --version 2.0.0` (check last version)

3. Add [BundlerMinifier](https://www.nuget.org/packages/BuildBundlerMinifier) nuget package

`> dotnet add package BuildBundlerMinifier --version 3.2.449`

4. Open "wwwroot" directory (if not exist - create it)

- Create "Components" dirictory (wwwroot/Modules)
- - Create "Total" dirictory   - is main component
- - - Create "content" dirictory (wwwroot/Modules/Total/content) - this is where images and other files will be stored
- - - - Add "Icon-32.png" (32x32px) (wwwroot/Modules/Total/content/Icon-32.png) - will be used as an Favicon icon
- - - - Add "Icon-48.png" (48x48px) (wwwroot/Modules/Total/content/Icon-38.png) - will be used as an Favicon icon
- - - - Add "Icon-64.png" (64x64px) (wwwroot/Modules/Total/content/Icon-64.png) - will be used as an Favicon icon
- - - - Add "Icon-70.png" (70x70px) (wwwroot/Modules/Total/content/Icon-70.png) - will be used as an Microsoft Tiles icon
- - - - Add "Icon-120.png" (120x120px) (wwwroot/Modules/Total/content/Icon-120.png) - will be used as an Apple Touch icon
- - - - Add "Icon-150.png" (150x150px) (wwwroot/Modules/Total/content/Icon-150.png) - will be used as an Microsoft Tiles icon
- - - - Add "Icon-152.png" (152x152px) (wwwroot/Modules/Total/content/Icon-152.png) - will be used as an Apple Touch icon
- - - - Add "Icon-167.png" (167x167px) (wwwroot/Modules/Total/content/Icon-167.png) - will be used as an Apple Touch icon
- - - - Add "Icon-180.png" (180x180px) (wwwroot/Modules/Total/content/Icon-180.png) - will be used as an Apple Touch icon
- - - - Add "Icon-192.png" (192x192px) (wwwroot/Modules/Total/content/Icon-192.png) - will be used as an Android and PWA icon
- - - - Add "Icon-310.png" (310x310px) (wwwroot/Modules/Total/content/Icon-310.png) - will be used as an Microsoft Tiles icon
- - - - Add "Icon-512.png" (512x512px) (wwwroot/Modules/Total/content/Icon-512.png) - will be used as an Android and PWA icon
- Create "UIRs" dirictory (wwwroot/UIRs) - For your Pages (UI Screens) - Required
- - Create Page (UI Screen) directory {Index} (wwwroot/UIRs/Index)
- - - Create file "script.js" (wwwroot/UIRs/Index/script.js) - Required. This is the file that will be called to build the page (UI Screen)
- - - Create file "style.css" (wwwroot/UIRs/Index/script.js) - File for storing styles
- - - Create "content" directory (wwwroot/UIRs/Index/content) - Вirectory for storing images and other page's content
- - - Create file "scheme.json" (wwwroot/UIRs/Index/scheme.json) - Page (UI Screen) scheme with primary information about the page and a list of links to external components (modules) that are used on this page (called script.js)


5. About scheme.json

```
{
    "title": "Main page",
    "description": "About Main page",
    "robot": "noindex",
    "modules": [ 
        { "path": "/Modules/ApiRtInk_MSAP", "toBundlerMinifier": true },
        { "path": "/Modules/HeadHtmlBox", "toBundlerMinifier": true },
        { "path": "/Modules/HeaderTextUI", "toBundlerMinifier": true },
        { "path": "/Modules/FooterInfHtmlBox", "toBundlerMinifier": true },
        { "path": "/Modules/ListGHtmlBox", "toBundlerMinifier": true },
        { "path": "/Modules/MoreButtonUI", "toBundlerMinifier": true }
        { "path": "/ExternalComponents/MomentjsCom/moment-timezone.min.js" }
    ]
}
```

title - The title (tag) of page that will be used in HEAD if not overridden
description - The description (meta) of page that will be used in HEAD if not overridden
robot - The <meta name="robots"> tag is an HTML element that provides instructions to search engine crawlers (or robots) about how to index and follow the links on a webpage.
modules - List of paths to components that will be called by the script.js of page (UI Screen).This list also helps determine which components should be bundled & minify with script & css pages. If you do not want to specify the path to all files, specify the path to the directory and we will get the paths to javascript & css files





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

var www = new Root(new ConfigM(
    IsDebug: isDebug,
    Url: https://localhost:1234,
    Name: "My Awesome App",
    NameShort: "AwesomeApp",
    Description: "A great app for managing your tasks.",
    CacheControlInSDefault: 60 * 60 * 3,
    PreconnectUrls: new List<string>() { "api.example.com", "api1.example.com", "api2.example.com" },
    GoogleFontsUrl: "https://fonts.googleapis.com/css2?family=Audiowide&family=Montserrat:wght@500;600;700&family=Nunito:wght@500;700&Mulish:wght@500&display=swap",
    GoogleAnalyticsCode: "G-2233KK44LS",
    YandexMetrikaCode: "88888888"
));

app.MapGet("/", async (context) => { await www.SendAsync(context, "i"); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/a/{urlShort}", async (HttpContext context, IHttpClientFactory httpClientFactory) => { await www.SendAsync(context, "i", await i_seo(context, httpClientFactory)); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/bookmarks", async (context) => { await www.SendAsync(context, "bookmarks"); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/error", async (context) => { await www.SendAsync(context, "Error"); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/i/{search}", async (HttpContext context, IHttpClientFactory httpClientFactory) => { await www.SendAsync(context, "i", await i_seo(context, httpClientFactory)); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/locs", async (context) => { await www.SendAsync(context, "Locs"); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/settings", async (context) => { await www.SendAsync(context, "Settings"); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/u/{login-search}", async (HttpContext context, IHttpClientFactory httpClientFactory) => { await www.SendAsync(context, "u", await u_seo(context, httpClientFactory)); }).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/users", async (context) => { await www.SendAsync(context, "users"); }).CacheOutput(ThreeHOutputCachePolicy);

```


## ASP.NET Core Project

An example of using Alga.wwwcore nuget package on a real ASP.NET Core project

web: [https://git.rt.ink](https://git.rt.ink)
git: [https://github.com/rtink-git/RtInkGit](https://github.com/rtink-git/RtInkGit/tree/main/RtInkGit/RtInkGit)