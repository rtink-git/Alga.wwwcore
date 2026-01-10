using System.Collections.Frozen;

namespace Alga.wwwcore.Core.HtmlGenerator.PageJsFile;

public class Req
{
    public required string Version { get; init; }
    public required string Key { get; init; }
    public required SchemesGenerator.PageModel PageModel { get; init; }
    public required FrozenDictionary<string, HashSet<string>> PageModules { get; init; }
    public required string DirectoryPath { get; init; }
}
