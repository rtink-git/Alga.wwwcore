namespace Alga.wwwcore;

public sealed class SeoPageOptions
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? Path { get; set; }
    public string? UrlCanonical { get; set; }
    public string? Robot { get; init; } = "noindex, nofollow";
    public string? ImageUrl { get; init; }
    public int? ImageWidth { get; init; }
    public int? ImageHeight { get; init; }
    public string? ImageEncodingFormat { get; init; }
    public decimal? ItemPrice { get; init; }
    public string? ItemCurrency { get; init; }
    public string? ItemAvailability { get; init; }
    public string? Lang { get; init; } = "en";
    public string? TypeOg { get; init; } = "website";
    public string? SchemaOrgsJson { get; init; }
}