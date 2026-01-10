namespace Alga.wwwcore.Core.FileGenerators.AppJs;

sealed class Req
{
    public required string DirectoryPath { get; init; }
    public required string Version { get; init; }
    public string? BackgroundColor { get; init; }
}
