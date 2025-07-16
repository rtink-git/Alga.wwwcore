using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
using System.Buffers;

namespace Alga.wwwcore;

/// <summary>
/// Generates a User Interface diagram for sending HTTP responses.
/// </summary>
public class Root
{

    /// <summary>
    /// The configuration manager used to retrieve App configuration.
    /// </summary>
    readonly Models.Config ConfigM;
    //readonly IHttpContextAccessor HttpContextAccessor;
    readonly ILogger? _Logger;
    readonly FrozenDictionary<string, Models.SchemeJsonM> Pages;
    readonly FrozenDictionary<string, HashSet<string>> PageModules;
    readonly Html _htmlGen;

    /// <summary>
    /// Initializes a new instance of the "Root" class
    /// </summary>
    /// <param name="config">A configuration object containing general App settings.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is null.</exception>
    public Root(Models.Config? config = null, bool isDebug = false, ILogger? logger = null)
    {
        ConfigM = config ?? throw new ArgumentNullException(nameof(config));
        ConfigM.IsDebug = isDebug;
        ConfigM.CurrentVersion = DateTime.UtcNow.ToString("yyyyMMddHHmm");
        _Logger = logger;
        _htmlGen = new Html(config);

        // Get pages schemes

        var schemes = new Schemes(config, _Logger);

        // Stores pre-rendered HTML page content in memory and immediately returns it to the client when requested.
        // This works if the page SEO is generated from scheme.json, if you generate seo pages yourself (for example, the query parameters have changed) the html page is not generated on the fly with additional allocations

        foreach (var i in schemes.Pages)
            i.Value.html = (new Html(ConfigM).GetBytes(i.Key, i.Value, schemes.PagesModules, null)).ToArray();

        new ManifestJson(this.ConfigM).Build();
        new AppJs(ConfigM).Create();
        new ServiceworkerJs().Build(ConfigM, schemes.Pages, schemes.PagesModules);

        Pages = schemes.Pages;
        PageModules = schemes.PagesModules;
    }

    //

    public void WriteHtml(IBufferWriter<byte> writer, string UISName, Seo? seoM = null)
    {
        if (Pages == null || !Pages.TryGetValue(UISName, out var pageVal)) return;

        if (seoM == null)
        {
            writer.Write(pageVal.html);
            return;
        }

        _htmlGen.WriteTo(writer, UISName, pageVal, PageModules, seoM);
    }
}