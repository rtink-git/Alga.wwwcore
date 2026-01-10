using System.Collections.Frozen;

namespace Alga.wwwcore.Core.FileGenerators.ServiceworkerJs;

sealed class Req
{
    public required FrozenDictionary<string, SchemesGenerator.PageModel>? Schemes { get; init; }
    public required string DirectoryPath { get; init; }
    public required string Version { get; init; }
    public string? OfflinePageUrl { get; init; }
    public string[]? CacheUrls { get; init; }
}
