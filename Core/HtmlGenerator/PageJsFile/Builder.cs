using NUglify;
using NUglify.JavaScript;
using System.Text;

namespace Alga.wwwcore.Core.HtmlGenerator.PageJsFile;

public class Builder
{
    const string _jsType = ".js";

    public void Create(Req req)
    {
        var scriptsToBundle = new List<string>(); // page script + page components scripts - collection

        string _minType = $".{req.Version}.min";
        var prefix = req.DirectoryPath.Contains("/") ? "/" : "\\";

        if (req.PageModel.modules?.Count > 0)
            foreach (var j in req.PageModel.modules)
                if (req.PageModules.TryGetValue(j, out var filesVal))
                    foreach (var fl in filesVal)
                    {
                        var scriptModuleFullPath = ReplacePrefix($"{req.DirectoryPath}{fl}", prefix);
                        if (File.Exists(scriptModuleFullPath))
                            if (scriptModuleFullPath.EndsWith(_jsType)) scriptsToBundle.Add(scriptModuleFullPath);
                    }

        var path = ReplacePrefix($"{req.DirectoryPath}{req.Key}", prefix) + prefix;

        // проверяем все .min файлы и удаляем их

        foreach (var file in Directory.GetFiles(path))
        {
            var name = new FileInfo(file).Name;
            if (name.Contains(".min.js"))
                File.Delete(file);
        }

        // --

        scriptsToBundle.Add($"{path}script.js");


        // Bundle page scripts in one script -> Minify + Compress

        var bundledJs = BundleFiles(scriptsToBundle);
        var jsMinify = Uglify.Js(bundledJs, _minifySettings);
        var jssp = $"script{_minType}{_jsType}";
        var jsMinPath = $"{path}{jssp}";
        File.WriteAllText(jsMinPath, jsMinify.Code);

        req.PageModel.script = $"{req.Key}/{jssp}";
    }

    string ReplacePrefix(string path, string prefix = "/") => path.Replace("\\", "/");

    string BundleFiles(List<string> list)
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

    static readonly CodeSettings _minifySettings = new()
    {
        PreserveImportantComments = false,
        TermSemicolons = true,
        MinifyCode = true,
        LocalRenaming = LocalRenaming.KeepAll
    };
}
