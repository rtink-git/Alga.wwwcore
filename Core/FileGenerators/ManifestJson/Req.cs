namespace Alga.wwwcore.Core.FileGenerators.ManifestJson;

sealed class Req
{
    public string DirectoryPath { get; set; }
    public string Version { get; set; }
    public string Name { get; set; }
    public string NameShort { get; set; }
    public string Description { get; set; }
    public string? BackgroundColor { get; set; }
    public string? ThemeColor { get; set; }
}

// sealed class Req
// {
//     public required string DirectoryPath { get; init; }
//     public required string Version { get; init; }
//     public required string Name { get; init; }
//     public required string NameShort { get; init; }
//     public required string Description { get; init; }
//     public string? BackgroundColor { get; init; }
//     public string? ThemeColor { get; init; }
// }