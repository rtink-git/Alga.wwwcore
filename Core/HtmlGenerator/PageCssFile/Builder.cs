using NUglify;
using NUglify.JavaScript;
using System.Text;

namespace Alga.wwwcore.Core.HtmlGenerator.PageCssFile;

public class Builder
{
    const string _cssType = ".css";

    public void Create(Req req)
    {
        string _minType = $".{req.Version}.min";

        var prefix = req.DirectoryPath.Contains("/") ? "/" : "\\";

        var stylesToBundle = new List<string>();  // page style + page components scripts - colleection

        // Filling collections

        if (req.PageModel.modules?.Count > 0)
            foreach (var j in req.PageModel.modules)
                if (req.PageModules.TryGetValue(j, out var filesVal))
                    foreach (var fl in filesVal)
                    {
                        var scriptModuleFullPath = ReplacePrefix($"{req.DirectoryPath}{fl}", prefix);
                        if (File.Exists(scriptModuleFullPath))
                            if (scriptModuleFullPath.EndsWith(_cssType)) stylesToBundle.Add(scriptModuleFullPath);
                    }

        var path = ReplacePrefix($"{req.DirectoryPath}{req.Key}", prefix) + prefix;

        // проверяем все .min файлы и удаляем их

        foreach (var file in Directory.GetFiles(path))
        {
            var name = new FileInfo(file).Name;
            if (name.Contains(".min.css"))
                File.Delete(file);
        }

        // --

        if (req.PageModel.style != null) stylesToBundle.Add($"{path}style.css");

        // Bundle page styles in one style -> Minify + Compress

        if (stylesToBundle.Count > 0)
        {
            var bundledCss = BundleFiles(stylesToBundle);
            var cssMinify = Uglify.Css(bundledCss);
            var csssp = $"style{_minType}{_cssType}";
            var cssMinPath = $"{path}{csssp}";
            File.WriteAllText(cssMinPath, cssMinify.Code);
            req.PageModel.style = $"{req.Key}/{csssp}";
        }
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
}