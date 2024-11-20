using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Alga.wwwcore;

/// <summary>
/// Generates a User Interface diagram for sending HTTP responses.
/// </summary>
public class Root
{
    /// <summary>
    /// The configuration manager used to retrieve App configuration.
    /// </summary>
    readonly ConfigM ConfigM;
    readonly ILogger? logger;
    /// <summary>
    /// A list of User Interface Screens schemes used for rendering views.
    /// </summary>
    readonly List<_UISchemes.M> UISchemes;

    /// <summary>
    /// Initializes a new instance of the "Root" class
    /// </summary>
    /// <param name="config">A configuration object containing general App settings.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is null.</exception>
    public Root(ConfigM config, ILoggerFactory? loggerFactory) {
        this.ConfigM = config ?? throw new ArgumentNullException(nameof(config));
        if(loggerFactory != null) logger = loggerFactory.CreateLogger<Root>();
        if(logger  != null) logger.LogInformation("Started");

        this.UISchemes = new _UISchemes(config, loggerFactory).Build();

        // Conditional initialization if debugging
        if(this.ConfigM.IsDebug) {
            new _BundleconfigJson(this.UISchemes).Build();
            new _ManifestJson(this.ConfigM).Build();
        }
    }

    /// <summary>
    /// Sends an HTTP response asynchronously with the specified UI screen scheme and optional SEO metadata.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
    /// <param name="UISName">The name of the UI screen scheme to be used for rendering the response.</param>
    /// <param name="seoM">Optional SEO metadata to include in the response.</param>
    /// <param name="cacheControlInS">Optional cache control setting (in seconds). Default is -1, in them mens but better add cache control. The default cache control setting (in seconds) for the entire project can be set in the configuration when initializing the class.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendAsync(HttpContext context, string UISName, SeoM? seoM = null, int cacheControlInS = -1) {
        var UIScheme = UISchemes.FirstOrDefault(i => i.UISName == UISName);
        if(UIScheme != null) await new _Response(ConfigM, UIScheme).Send(context, seoM, cacheControlInS);
    }
}