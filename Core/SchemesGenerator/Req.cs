namespace Alga.wwwcore.Core.SchemesGenerator;

sealed class Req
{
    public bool IsDebug { get; init; }
    public required string DirectoryPath { get; init; }
    public required string Version { get; init; }
}
