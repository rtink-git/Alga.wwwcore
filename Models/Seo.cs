namespace Alga.wwwcore.Models;

public sealed class Seo
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Path { get; set; }
    public string? UrlCanonical { get; set; }
    public string? Robot { get; set; }
    public string? ImageUrl { get; set; }
    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
    public string? ImageEncodingFormat { get; set; }
    public decimal? ItemPrice { get; set; }
    public string? ItemCurrency { get; set; }
    public string? ItemAvailability { get; set; }
    public string? Lang { get; set; }
    public string? TypeOg { get; set; }
    public string? SchemaOrgsJson { get; set; }
    // public SchemaOrg[]? SchemaOrgs { get; set; }
}