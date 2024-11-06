# Alga.wwwcore

ASP.NET Core minimal api programming approach, the purpose of which is to facilitate the development of web applications and at the same time concentrate on fine-tuning the application


### How does this work. Step by step

1. Create ASP.NET Core project or open an existing project

2. Add [Alga.wwwcore](https://www.nuget.org/packages/Alga.wwwcore) nuget package

`> dotnet add package Alga.wwwcore --version 1.0.11`

3. Add [BundlerMinifier](https://www.nuget.org/packages/BuildBundlerMinifier) nuget package

`> dotnet add package BuildBundlerMinifier --version 3.2.449`

4. Open wwwroot directory (if not exist - create it)
- Create "UIs" dirictory (wwwroot/UIs)
- - Create "Components" dirictory (wwwroot/UIs/Components)
- - Create "ExternalComponents" dirictory (wwwroot/UIs/ExternalComponents)
- - Create "UIRs" dirictory (wwwroot/UIs/UIRs)

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
    name: "Fohouse Store",
    url: (isDebug) ? "https://localhost:6543" : "https://example.com",
    GoogleFontsUrl: "https://fonts.googleapis.com/css2?family=Audiowide&family=Montserrat:wght@500;600;700&family=Nunito:wght@500;700&Mulish:wght@500&display=swap",
    UICacheControlInSDefault: ForhouseWeb.Constants.threeHInSecForCache
));

app.MapGet("/", async (context) => { await uis.Response(context, uis.i(), uis.SEO("Project", "")); }).WithRequestTimeout(S5TimeoutPolicy).CacheOutput(ThreeHOutputCachePolicy);
app.MapGet("/about", async (context) => { await uis.Response(context, uis.about(), uis.SEO("О компании", "")); }).WithRequestTimeout(S5TimeoutPolicy).CacheOutput(ThreeHOutputCachePolicy);
```