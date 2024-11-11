using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Alga.wwwcore;

/// <summary>
/// Abstract Class for constructing a "user interface" diagram
/// </summary>
public abstract class SchemeB
{
    ConfigM ConfigM { get; }

    public SchemeB(ConfigM config) {
        this.ConfigM = config;
        if(this.ConfigM.IsDebug) {
            new _BundleconfigJson(this).Build();
            new _ManifestJson(this.ConfigM).Build();
        }
    }

    /// <summary>
    /// User Interface Screen - model. In web applications this is a "page"
    /// </summary>
    public record ScreenM (
        /// <summary>
        /// The name of the method must match the name of the directory in wwwroot/Modules/{MethodName}
        /// </summary>
        MethodBase? Method,
        DirСontainFilesEnum? DirСontains = DirСontainFilesEnum.JsAndCss,
        List<ModuleM>? ModuleMs = null,
        List<ModuleExtM>? ModuleExts = null,
        ConfigM? Config = null
    ){ 
        public async Task SendAsync(HttpContext context, SeoM? seoM = null, int cacheControlInS = -1) { if(Config != null) await new _Response(Config).Send(context, this, seoM, cacheControlInS); } 
    }

    /// <summary>
    /// User Interface Module. It is: Button, Checkbox, Menu, Modal windows, Lists, Apis modules, ... and other executable code
    /// </summary>
    public record ModuleM (
        /// <summary>
        /// The name of the method must match the name of the directory in wwwroot/Modules/{MethodName}
        /// </summary>
        MethodBase? Method,
        DirСontainFilesEnum? DirСontains = DirСontainFilesEnum.JsAndCss
    );

    public record ModuleExtM (
        MethodBase? Method,
        string Url,
        FileTypeEnum FileType = FileTypeEnum.Js
    );


    public enum FileTypeEnum {
        Css,
        Js
    }


    /// <summary>
    /// Directory Сontain next Files Enum
    /// </summary>
    public enum DirСontainFilesEnum { 
        /// <summary>
        /// Directory contain next files:
        /// script.js - wwwroot/Modules/{MethodName}/script.js
        /// style.css - wwwroot/Modules/{MethodName}/style.ss
        /// </summary>
        JsAndCss, 
        /// <summary>
        /// Directory contain next files:
        /// script.js - wwwroot/Modules/{MethodName}/script.js
        /// </summary>
        JsOnly, 
        /// <summary>
        /// Directory contain next files:
        /// style.css - wwwroot/Modules/{MethodName}/style.ss
        /// </summary>
        CssOnly 
    }
}