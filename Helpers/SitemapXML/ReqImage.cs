namespace Alga.wwwcore.Helpers.SitemapXML;

public sealed class ReqImage
{
    public required string Url { get; init; }
    public string? Caption { get; init; }
    public string? Title { get; init; }
    public string? License { get; init; }
}
