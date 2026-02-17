using System.Text.Json;

namespace Alga.wwwcore.Core.SchemesGenerator;

sealed class Builder
{
    const string _jsType = ".js";
    const string _cssType = ".css";
    const string scriptName = $"script{_jsType}";
    const string styleName = $"style{_cssType}";
    const string schemeName = "scheme.json";

    public Res Do(Req req)
    {
        var prefix = req.DirectoryPath.Contains("/") ? "/" : "\\";

        // 1. Get a collection of paths to all scheme.json (page scheme) files found in the wwwroot folder. 

        var files = Directory.GetFiles(req.DirectoryPath, schemeName, SearchOption.AllDirectories);

        // 2. Create a frozen collection of pages with schemes

        var pages = new Dictionary<string, PageModel>();

        foreach (var i in files)
        {
            var schemeJsonM = ReadSchemeJson(i);

            if (schemeJsonM == null) schemeJsonM = new PageModel();

            // Specifies the path to the directory within wwwroot containing the page schema JSON file (scheme.json)

            var filePath = i.Substring(req.DirectoryPath.Length, i.Length - req.DirectoryPath.Length - schemeName.Length - 1);

            // Get full path to page script

            var scriptFullPath = $"{req.DirectoryPath}{filePath}{prefix}{scriptName}";
            if (File.Exists(scriptFullPath)) schemeJsonM.script = ReplacePrefix(scriptFullPath.Substring(req.DirectoryPath.Length));
            else continue;

            // Get full path to page style

            var styleFullPath = $"{req.DirectoryPath}{filePath}{prefix}{styleName}";
            if (File.Exists(styleFullPath))
                schemeJsonM.style = ReplacePrefix(styleFullPath.Substring(req.DirectoryPath.Length));

            var key = filePath.Replace("\\", "/");

            schemeJsonM.script = $"{key}/script.js";

            // Add scheme in memory collection

            pages.TryAdd(key, schemeJsonM);
        }

        // 3. Creates a pages modules schemes collection

        var pageModules = new Dictionary<string, HashSet<string>>();

        foreach (var i in pages)
            if (i.Value.modules?.Count > 0)
                foreach (var j in i.Value.modules)
                {
                    if (pageModules.ContainsKey(j)) continue;

                    var hs = new HashSet<string>();

                    var modulePath = j.Replace("\\", prefix);

                    // Get full path to module scriipt

                    var scriptModuleFullPath = $"{req.DirectoryPath}{modulePath}{prefix}{scriptName}";
                    if (File.Exists(scriptModuleFullPath))
                        hs.Add(ReplacePrefix(scriptModuleFullPath.Substring(req.DirectoryPath.Length)));

                    // Get full path to module style

                    var styleModuleFullPath = $"{req.DirectoryPath}{modulePath}{prefix}{styleName}";
                    if (File.Exists(styleModuleFullPath))
                        hs.Add(ReplacePrefix(styleModuleFullPath.Substring(req.DirectoryPath.Length)));

                    // Add module in the collection

                    if (hs.Count > 0) pageModules.Add(j, hs);
                }

        return new Res(pages, pageModules);
    }

    // Method for reading the schema from a JSON file.
    PageModel? ReadSchemeJson(string filePath)
    {
        using var fs = File.OpenRead(filePath);
        return JsonSerializer.Deserialize<PageModel>(fs, PageModelJsonContext.Default.PageModel);
    }

    string ReplacePrefix(string path, string prefix = "/") => path.Replace("\\", "/");
}