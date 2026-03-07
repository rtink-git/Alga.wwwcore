namespace Alga.wwwcore;

public sealed class SeoPageOptions
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Path { get; set; }
    public string? UrlCanonical { get; set; }
    public string? Robot { get; set; } = "noindex, nofollow";
    public ImageDto? Image { get; set; }
    public ImageDto? Icon32 { get; set; }
    public ImageDto? Icon180 { get; set; }
    public ImageDto? Icon192 { get; set; }
    public decimal? ItemPrice { get; set; }
    public string? ItemCurrency { get; set; }
    public string? ItemAvailability { get; set; }
    public string? Lang { get; set; } = "en";
    public string? TypeOg { get; set; } = "website";
    public string? SchemaOrgsJson { get; set; }
}

public sealed class ImageDto
{
    public string? Url { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? EncodingFormat { get; set; }
}