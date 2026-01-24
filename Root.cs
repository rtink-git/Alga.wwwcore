using System.Buffers;

namespace Alga.wwwcore;

public sealed class Root
{
    public ClientSettings ClientSettings;

    Core.IInitializer _coreInitializer { get; }

    public Root(ClientSettings clientSettings, bool isDebug)
    {
        if (clientSettings == null) throw new ArgumentException(nameof(clientSettings));

        clientSettings.Validate();

        ClientSettings = clientSettings;

        _coreInitializer = new Core.Initializer(clientSettings, isDebug);
    }

    public void WriteHtml(IBufferWriter<byte> writer, string UISName, SeoPageOptions seoPageOptions, string? pageModelAsJson = null) => new Core.UseCases.WriteHtml(_coreInitializer).Do(writer, UISName, seoPageOptions, pageModelAsJson);
}