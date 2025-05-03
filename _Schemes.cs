using Microsoft.Extensions.Logging;
using System.Text.Json;
using NUglify;
using NUglify.JavaScript;

namespace Alga.wwwcore;

class _Schemes {
    readonly ILogger? _Logger;
    public List<Models.SchemeJsonM> List { get; set; } = new();
    public Dictionary<string, HashSet<string>> Modules = new ();

    public _Schemes(ILogger? logger) => _Logger = logger;

    public async Task Build() {
        _Logger?.LogInformation("Construction of the web project scheme - Started");

        var currentDirPathFull = Directory.GetCurrentDirectory();

        var wwwroot_dir = "wwwroot";
        var wwwroot_path_full = Path.Combine(currentDirPathFull, wwwroot_dir);
        if(!Directory.Exists(wwwroot_path_full)) { _Logger?.LogError($"Directory must exist (this is the main public directory of the web project): {wwwroot_dir}"); return; }

        var files = Directory.GetFiles(wwwroot_path_full, "*.*", SearchOption.AllDirectories);

        var modulesPaths = new HashSet<string>();

        var jsType = ".js";
        var cssType = ".css";

        var scriptName = $"script{jsType}";
        var styleName = $"style{cssType}";
        var schemeName = "scheme.json";

        foreach(var i in files) 
            if(i.EndsWith(schemeName)) {
                var name = i.Substring(wwwroot_path_full.Length, i.Length - wwwroot_path_full.Length - schemeName.Length - 1);

                // get users scheme

                Models.SchemeJsonM? schemeJsonM = null;
                try { 
                    var jsonString = File.ReadAllText(i); 
                    schemeJsonM = JsonSerializer.Deserialize<Models.SchemeJsonM>(jsonString); 
                    if(schemeJsonM?.modules != null)
                        foreach(var module in schemeJsonM.modules)
                            modulesPaths.Add(module);
                } catch { }

                if(schemeJsonM == null) schemeJsonM = new Models.SchemeJsonM();

                // script

                var script_path = schemeJsonM.script ?? $"{name}/{scriptName}";
                var script_path_full = $"{wwwroot_path_full}{script_path}";
                if(!File.Exists(script_path_full)) { _Logger?.LogError($"The directory should contain \"{scriptName}\" which builds the DOM of the page in {i}"); continue; }
                schemeJsonM.script = script_path;
                await Minify(script_path_full);

                // style

                var style_path = schemeJsonM.style ?? $"{name}/{styleName}";
                var style_path_full = $"{wwwroot_path_full}{style_path}";
                if(!File.Exists(style_path_full)) _Logger?.LogWarning($"Are you sure you didn't forget to add the \"{styleName}\" in {i}");
                else { schemeJsonM.style = style_path; await Minify(style_path_full); }

                List.Add(schemeJsonM);
            }

        foreach(var i in modulesPaths) {
            var hs = new HashSet<string>();

            var scriptPath = $"{i}/{scriptName}";
            var scriptFullPath = $"{wwwroot_path_full}{scriptPath}";
            if(File.Exists(scriptFullPath)) {
                hs.Add($"{scriptPath}");
                await Minify(scriptFullPath);
            }

            string stylePath = $"{i}/style.css";
            var styleFullPath = $"{wwwroot_path_full}{i}/{styleName}";
            if(File.Exists(styleFullPath)) {
                hs.Add($"{stylePath}");
                await Minify(styleFullPath);
            }
            
            if(hs.Count > 0) Modules.Add(i, hs);
        }
    }

    async Task<bool> Minify(string path) {
        try {
            var str = await File.ReadAllTextAsync(path);

            var jsType = ".js";
            var cssType = ".css";
            var minType = ".min";
            if(path.EndsWith(jsType)) {
                var settings = new CodeSettings {
                    PreserveImportantComments = false,
                    TermSemicolons = true,
                    MinifyCode = true,
                    LocalRenaming = LocalRenaming.KeepAll, // Не переименовывает локальные переменные (включая классы)
                };

                var ms = Uglify.Js(str, settings);
                if(!ms.HasErrors)
                    await File.WriteAllTextAsync($"{path.Substring(0, path.Length - jsType.Length)}{minType}{jsType}", ms.Code);
            } else if(path.EndsWith(cssType)) {
                var ms = Uglify.Css(str);
                if(!ms.HasErrors)
                    await File.WriteAllTextAsync($"{path.Substring(0, path.Length - cssType.Length)}{minType}{cssType}", ms.Code);
            }
        } catch { }

        return false;
    }
}