using System.Text.Json;

namespace Alga.wwwcore;

/// <summary>
/// Class responsible for generating a bundle configuration JSON file that defines how JavaScript and CSS files should be bundled and minified.
/// </summary>
class _BundleconfigJson {
    /// <summary>
    /// A list of UI schemes that define the structure and assets of the UI.
    /// </summary>
    List<_UISchemes.M> _UISchemes { get; }

    /// <summary>
    /// Constructor: Initializes the class with a list of UI schemes. Throws an exception if the list is null to prevent invalid input.
    /// </summary>
    /// <param name="_uISchemes"></param>
    /// <exception cref="ArgumentNullException">List of UI schemes</exception>
    public _BundleconfigJson(List<_UISchemes.M> _uISchemes) => this._UISchemes = _uISchemes ?? throw new ArgumentNullException(nameof(_uISchemes));  // Защита от null

    /// <summary>
    /// Main method that orchestrates the bundling process and writes the final output to a JSON file.
    /// </summary>
    internal void Build() {
        var l = new List<BundleM>();
        ProcessScreenM(l);
        
        var filePath = Path.Combine(Environment.CurrentDirectory, "bundleconfig.json");
        using (FileStream createStream = File.Create(filePath)) { JsonSerializer.Serialize(createStream, l); }
    }

    /// <summary>
    /// Method to process each UI scheme, generate bundle configurations, and add them to the list.
    /// </summary>
    /// <param name="models">Bundle configurations</param>
    void ProcessScreenM(List<BundleM> models)
    {
        foreach (var i in this._UISchemes) {
            var scripts = GetMinifiedFiles(i.ModulesJs);
            var styles = GetMinifiedFiles(i.ModulesCss);

            var outputDir = Path.Combine("wwwroot", "UISs", i.UISName);
            scripts.Add($"{outputDir}/script.js");
            styles.Add($"{outputDir}/style.css");

            var minifyModel = new MinifyM(true, true);

            models.Add(new BundleM($"{outputDir}/script.min.js", scripts, minifyModel));
            models.Add(new BundleM($"{outputDir}/style.min.css", styles, minifyModel));
        }
    }

    /// <summary>
    /// Helper method to get the list of files marked for minification from the provided modules. It filters the modules to include only those that are marked for bundling and minification.
    /// </summary>
    /// <param name="modules">The list of UI modules (JavaScript or CSS) to be processed.</param>
    /// <returns>A list of file paths for the modules marked for minification, or an empty list if no modules are selected for minification.</returns>
    List<string> GetMinifiedFiles(IEnumerable<_UISchemes.Module>? modules) => modules?.Where(m => m.toBundlerMinifier == true).Select(m => "wwwroot" + m.path).ToList() ?? new List<string>();

    /// <summary>
    /// Record that represents the configuration for a bundle. Includes the output file name, the list of input files, and the minification options.
    /// </summary>
    /// <param name="outputFileName"></param>
    /// <param name="inputFiles"></param>
    /// <param name="minify"></param>
    record BundleM ( string? outputFileName, List<string>? inputFiles, MinifyM? minify );
    /// <summary>
    /// Record that holds the minification settings for a bundle. Defines whether minification is enabled and if local variables should be renamed.
    /// </summary>
    /// <param name="enabled"></param>
    /// <param name="renameLocals"></param>
    record MinifyM( bool enabled, bool renameLocals );
}