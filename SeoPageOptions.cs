namespace Alga.wwwcore;

public sealed class SeoPageOptions
{
    public string Title { get; set; } = null!; // required
    public string? Description { get; set; }
    public string? Path { get; set; }
    public string? UrlCanonical { get; set; }
    public string? Robot { get; set; } = "noindex, nofollow";
    public string? ImageUrl { get; set; }
    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
    public string? ImageEncodingFormat { get; set; }
    public decimal? ItemPrice { get; set; }
    public string? ItemCurrency { get; set; }
    public string? ItemAvailability { get; set; }
    public string? Lang { get; set; } = "en";
    public string? TypeOg { get; set; } = "website";
    public string? SchemaOrgsJson { get; set; }
}