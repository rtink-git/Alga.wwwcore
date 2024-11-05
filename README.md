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


### Updates

- Version 1.1.1: Nuget package structure has been updated and minified
- Version 1.1.1: Bundleconfig.json file generated automatically
- Version 1.0.11: A Config.cs file has been added to set basic parameters
- Version 1.0.10: A critical error that was associated with the operation of the pocket in the DEBUG & RELEASE status has been fixed
- Version 1.0.0 The ability to add configuration rules during initialization has been added
- Version 1.0.8: Base directories are authomatically added to "wwwroot"