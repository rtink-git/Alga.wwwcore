using System.Runtime.CompilerServices;
using System.IO.Pipelines;

namespace Alga.wwwcore;

public sealed class Root
{
    public ClientSettings ClientSettings;

    Core.IInitializer _coreInitializer { get; }
    Core.UseCases.WriteHtml? _htmlWriter;

    public Root(ClientSettings clientSettings, bool isDebug)
    {
        if (clientSettings == null) throw new ArgumentException(nameof(clientSettings));

        // clientSettings.Validate();

        ClientSettings = clientSettings;

        _coreInitializer = new Core.Initializer(clientSettings, isDebug);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteHtml(PipeWriter writer, string UISName, SeoPageOptions seoPageOptions, string? pageModelAsJson = null)
    {
        var writerImpl = _htmlWriter ??= new Core.UseCases.WriteHtml(_coreInitializer);
        writerImpl.Do(writer, UISName, seoPageOptions, pageModelAsJson);
    }
}