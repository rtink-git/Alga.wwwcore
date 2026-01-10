namespace Alga.wwwcore.Core.FileGenerators.ManifestJson;

sealed class Req
{
    public required string DirectoryPath { get; init; }
    public required string Version { get; init; }
    public required string Name { get; init; }
    public required string NameShort { get; init; }
    public required string Description { get; init; }
    public string? BackgroundColor { get; init; }
    public string? ThemeColor { get; init; }
}