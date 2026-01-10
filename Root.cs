using System.Buffers;
using Microsoft.Extensions.Logging;

namespace Alga.wwwcore;

public class Root
{
    readonly ILogger? _logger;
    Core.IInitializer _coreInitializer { get; }

    public Root(ClientOptions clientOptions, bool isDebug, ILogger? logger)
    {
        ClientOptionsValidator.Do(clientOptions);

        _logger = logger;
        _coreInitializer = new Core.Initializer(clientOptions, isDebug);
        _coreInitializer.Do();
    }

    public void WriteHtml(IBufferWriter<byte> writer, string UISName, SeoPageOptions seoPageOptions, string? pageModelAsJson = null) => new Core.UseCases.WriteHtml(_coreInitializer).Do(writer, UISName, seoPageOptions, pageModelAsJson);
}