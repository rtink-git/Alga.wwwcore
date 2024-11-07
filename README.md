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


5. Create "wwwcore.cs" file (Project/wwwcore.cs)

```
using System.Reflection;
using Alga.wwwcore;

public class Wwwcore: UIsBase
{
    public Wwwcore(ConfigModel config) : base(config) { if(config.IsDebug) BundleconfigJsonRebuild(typeof(Wwwcore)); }

    // -- UI Components - groups.

    // -- Pages UI. Consist of the page code and components that are called from it. For release versions all components(+page script & style) are collected into two files: /UIRs/[current page]/[script.min.js & style.min.css]
    // -- Path: wwwroot/UIRs

    // -- UI Components
    // -- Path: wwwroot/Components

    // -- ExternalComponents. External page components that are not recomended to be combined into one main file
    // -- Path: wwwroot/ExternalComponents/
}

```

6. Program.cs & "Alga.wwwcore" nuget package

```
var isDebug = true;

if (!app.Environment.IsDevelopment())
{
    isDebug = false;
    ...
}

...

var uis = new Wwwcore(new Alga.wwwcore.UIsBase.ConfigModel(
    IsDebug: isDebug,
    name: "My Store",
    url: (isDebug) ? "https://localhost:6543" : "https://example.com",
    GoogleFontsUrl: "https://fonts.googleapis.com/css2?family=Audiowide&family=Montserrat:wght@500;600;700&family=Nunito:wght@500;700&Mulish:wght@500&display=swap",
    UICacheControlInSDefault: ForhouseWeb.Constants.threeHInSecForCache
));

app.MapGet("/", async (context) => { await uis.Response(context, uis.i(), uis.SEO("Project", "")); }).WithRequestTimeout(S5TimeoutPolicy).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/about", async (context) => { await uis.Response(context, uis.about(), uis.SEO("О компании", "")); }).WithRequestTimeout(S5TimeoutPolicy).CacheOutput(ThreeHOutputCachePolicy);
```