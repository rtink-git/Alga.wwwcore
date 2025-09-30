namespace Alga.wwwcore.Models;

public sealed class Seo
{
    public string? Title { get; set; }
    public string? Path { get; set; }
    public string? UrlCanonical { get; set; }
    public string? Robot { get; set; }
    public string? ImageUrl { get; set; }
    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
    public string? Lang { get; set; }
    public SchemaOrg? SchemaOrg { get; set; }
}