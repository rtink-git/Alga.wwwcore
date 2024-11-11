using System.Reflection;
using System.Text.Json;

namespace Alga.wwwcore;

class _BundleconfigJson {
    object Obj;
    public _BundleconfigJson(object obj) => this.Obj = obj;

    internal void Build() {
        var l = new List<BundleM>();
        var methods = this.Obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.ReturnType == typeof(SchemeB.ScreenM));

        foreach(var i in methods)
            if (i.Invoke(this.Obj, null) is SchemeB.ScreenM screenM)
                ProcessScreenM(screenM, l);

        var filePath = Path.Combine(Environment.CurrentDirectory, "bundleconfig.json");
        using (FileStream createStream = File.Create(filePath)) { JsonSerializer.Serialize(createStream, l); }
    }

    void ProcessScreenM(SchemeB.ScreenM screenM, List<BundleM> models)
    {
        if (screenM.ModuleMs == null) return;

        var scripts = new List<string>();
        var styles = new List<string>();

        foreach (var module in screenM.ModuleMs)
            if (module.Method != null) {
                var moduleUrl = Path.Combine("wwwroot", "Modules", module.Method.Name);
                bool containsJsAndCss = module.DirСontains == SchemeB.DirСontainFilesEnum.JsAndCss;
                if (containsJsAndCss || module.DirСontains == SchemeB.DirСontainFilesEnum.JsOnly)
                        scripts.Add(moduleUrl + "/script.js");
                if (containsJsAndCss || module.DirСontains == SchemeB.DirСontainFilesEnum.CssOnly)
                        styles.Add(moduleUrl + "/style.css");
            }
        
        var minifyModel = new MinifyM(true, true);

        if (screenM.Method != null) {
            var outputDir = Path.Combine("wwwroot", "UISs", screenM.Method.Name);
            models.Add(new BundleM (outputFileName: outputDir + "/script.min.js", inputFiles: scripts, minify: minifyModel));
            models.Add(new BundleM (outputFileName: outputDir + "/style.min.css", inputFiles: styles, minify: minifyModel));
        }
    }

    record BundleM ( string? outputFileName, List<string>? inputFiles, MinifyM? minify );
    record MinifyM( bool enabled, bool renameLocals );
}