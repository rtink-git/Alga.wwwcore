namespace Alga.wwwcore.Helpers.SitemapXML;

public sealed class ReqVideo
{
    public required string ThumbnailUrl { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string ContentUrl { get; init; }

    public DateTime? PublicationDate { get; init; }
    public int? DurationSeconds { get; init; }
    public string? FamilyFriendly { get; init; }
}