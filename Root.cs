using System.Buffers;

namespace Alga.wwwcore;

public class Root
{
    Core.IInitializer _coreInitializer { get; }


    public Root(ClientOptions clientOptions, bool isDebug)
    {
        var getClientOptionsRes = Operations.GetClientOptions.H.Do(clientOptions);

        _coreInitializer = new Core.Initializer(getClientOptionsRes, isDebug);
        _coreInitializer.Do();
    }

    public void WriteHtml(IBufferWriter<byte> writer, string UISName, SeoPageOptions seoPageOptions, string? pageModelAsJson = null) => new Core.UseCases.WriteHtml(_coreInitializer).Do(writer, UISName, seoPageOptions, pageModelAsJson);
}