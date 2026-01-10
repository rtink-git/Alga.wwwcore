namespace Alga.wwwcore.Core.HtmlGenerator.SeoMetaGenerator;

sealed public class Req
{
    public required string Url { get; init; }

    public required string NameShort { get; init; }

    public string? TwitterSite { get; init; }

    public required SeoPageOptions SeoPageOptions { get; init; }
}
