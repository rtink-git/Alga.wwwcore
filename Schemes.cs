using Microsoft.Extensions.Logging;
using NUglify;
using NUglify.JavaScript;
using System.Text.Json;
using System.Collections.Frozen;
using System.Text;

namespace Alga.wwwcore;

// The Schemes class is responsible for processing page schemas and making them production-ready.
class Schemes
{
    Models.Config _config;
    readonly ILogger? _logger;
    readonly string _minType;
    readonly string _wwwrootPathFull;
    readonly string _prefix; // Prefix that the system uses: windows - \, macos - /

    public FrozenDictionary<string, Models.SchemeJsonM> Pages { get; private set; } = FrozenDictionary<string, Models.SchemeJsonM>.Empty; // Frozen page schemes collection. All directories in wwwroot that contain scheme.json
    public FrozenDictionary<string, HashSet<string>> PagesModules { get; private set; } = FrozenDictionary<string, HashSet<string>>.Empty; // Frozen pages modules schemes collection. Frozen collection of components that use the pages.

    public Schemes(Models.Config config, ILogger? logger)
    {
        _config = config;
        _logger = logger;

        _minType = $".{_config.CurrentVersion}.min";

        // Defines the path to the public wwwroot directory.

        _wwwrootPathFull = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        if (!Directory.Exists(_wwwrootPathFull))
        {
            throw new DirectoryNotFoundException($"Required wwwroot directory not found at: {_wwwrootPathFull}");
        }

        _prefix = _wwwrootPathFull.Contains("/") ? "/" : "\\";

        Build();
    }

    const string _jsType = ".js";
    const string _cssType = ".css";

    static readonly CodeSettings _minifySettings = new()
    {
        PreserveImportantComments = false,
        TermSemicolons = true,
        MinifyCode = true,
        LocalRenaming = LocalRenaming.KeepAll
    };


    // Generates product page schemas in memory based on the processing of .json files (from wwwroot) that define page schemas.
    void Build()
    {
        const string nm = $"Schemes.{nameof(Build)}()";

        if (_config.IsDebug) _logger?.LogInformation($"{nm} Generates a product page schemas in memory - Started");

        const string schemeName = "scheme.json";

        // 1. Get a collection of paths to all scheme.json (page scheme) files found in the wwwroot folder. 

        var files = Directory.GetFiles(_wwwrootPathFull, schemeName, SearchOption.AllDirectories);

        const string scriptName = $"script{_jsType}";
        const string styleName = $"style{_cssType}";

        // 2. Create a frozen collection of pages with schemes

        var pages = new Dictionary<string, Models.SchemeJsonM>();

        foreach (var i in files)
        {
            var schemeJsonM = ReadSchemeJson(i);

            if (schemeJsonM == null) schemeJsonM = new Models.SchemeJsonM();

            // Specifies the path to the directory within wwwroot containing the page schema JSON file (scheme.json)

            var filePath = i.Substring(_wwwrootPathFull.Length, i.Length - _wwwrootPathFull.Length - schemeName.Length - 1);

            // Get full path to page script

            var scriptFullPath = $"{_wwwrootPathFull}{filePath}{_prefix}{scriptName}";
            if (File.Exists(scriptFullPath)) schemeJsonM.script = ReplacePrefix(scriptFullPath.Substring(_wwwrootPathFull.Length));
            else continue;

            // Get full path to page style

            var styleFullPath = $"{_wwwrootPathFull}{filePath}{_prefix}{styleName}";
            if (File.Exists(styleFullPath))
                schemeJsonM.style = ReplacePrefix(styleFullPath.Substring(_wwwrootPathFull.Length));

            // Add scheme in memory collection

            pages.TryAdd(filePath.Replace("\\", "/"), schemeJsonM);
        }

        Pages = pages.ToFrozenDictionary();

        // 3. Creates a pages modules schemes collection

        var pageModules = new Dictionary<string, HashSet<string>>();

        foreach (var i in pages)
            if (i.Value.modules?.Count > 0)
                foreach (var j in i.Value.modules)
                {
                    if (pageModules.ContainsKey(j)) continue;

                    var hs = new HashSet<string>();

                    var modulePath = j.Replace("\\", _prefix);

                    // Get full path to module scriipt

                    var scriptModuleFullPath = $"{_wwwrootPathFull}{modulePath}{_prefix}{scriptName}";
                    if (File.Exists(scriptModuleFullPath))
                        hs.Add(ReplacePrefix(scriptModuleFullPath.Substring(_wwwrootPathFull.Length)));

                    // Get full path to module style

                    var styleModuleFullPath = $"{_wwwrootPathFull}{modulePath}{_prefix}{styleName}";
                    if (File.Exists(styleModuleFullPath))
                        hs.Add(ReplacePrefix(styleModuleFullPath.Substring(_wwwrootPathFull.Length)));

                    // Add module in the collection

                    if (hs.Count > 0) pageModules.Add(j, hs);
                }

        PagesModules = pageModules.ToFrozenDictionary();

        if (_config.IsDebug) return;

        // 4. Bundle scripts and page components into one file (also for styles). Minify. Create compressed copies in gzip and brotly

        foreach (var i in pages)
        {
            var scriptsToBundle = new List<string>(); // page script + page components scripts - colleection
            var stylesToBundle = new List<string>();  // page style + page components scripts - colleection

            // Filling collections

            if (i.Value.modules?.Count > 0)
                foreach (var j in i.Value.modules)
                    if (pageModules.TryGetValue(j, out var filesVal))
                        foreach (var fl in filesVal)
                        {
                            var scriptModuleFullPath = ReplacePrefix($"{_wwwrootPathFull}{fl}", _prefix);
                            if (File.Exists(scriptModuleFullPath))
                            {
                                if (scriptModuleFullPath.EndsWith(".js")) scriptsToBundle.Add(scriptModuleFullPath);
                                if (scriptModuleFullPath.EndsWith(".css")) stylesToBundle.Add(scriptModuleFullPath);
                            }
                        }

            var path = ReplacePrefix($"{_wwwrootPathFull}{i.Key}", _prefix) + _prefix;

            scriptsToBundle.Add($"{path}script.js");
            if (i.Value.style != null) stylesToBundle.Add($"{path}style.css");

            // Bundle page scripts in one script -> Minify + Compress

            var bundledJs = BundleFiles(scriptsToBundle);
            var jsMinify = Uglify.Js(bundledJs, _minifySettings);
            var jssp = $"script{_minType}{_jsType}";
            var jsMinPath = $"{path}{jssp}";
            File.WriteAllText(jsMinPath, jsMinify.Code);
            i.Value.script = $"{i.Key}/{jssp}";

            // Bundle page styles in one style -> Minify + Compress

            if (stylesToBundle.Count > 0)
            {
                var bundledCss = BundleFiles(stylesToBundle);
                var cssMinify = Uglify.Css(bundledCss);
                var csssp = $"style{_minType}{_cssType}";
                var cssMinPath = $"{path}{csssp}";
                File.WriteAllText(cssMinPath, cssMinify.Code);
                i.Value.style = $"{i.Key}/{csssp}";
            }
        }
    }

    // Method for reading the schema from a JSON file.
    Models.SchemeJsonM? ReadSchemeJson(string filePath)
    {
        try
        {
            var jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Models.SchemeJsonM>(jsonString);
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error reading {filePath}: {ex.Message}");
            return null;
        }
    }

    static string BundleFiles(List<string> list)
    {
        var sb = new StringBuilder();

        foreach (var file in list)
        {
            var fileContent = File.ReadAllText(file);
            sb.AppendLine(fileContent);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    static string ReplacePrefix(string path, string prefix = "/") => path.Replace("\\", "/");
 }