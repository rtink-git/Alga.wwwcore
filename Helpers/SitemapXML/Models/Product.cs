namespace Alga.wwwcore.Helpers.SitemapXML.Models;

public sealed class Product
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required double Price { get; init; }
    public required string Currency { get; init; }

    public string? Description { get; init; }
    public string? Image { get; init; }
    
    public string? Brand { get; init; }
    public string? Category { get; init; }
    public string? Availability { get; init; }
    public string? Condition { get; init; }
    public string? Gtin { get; init; }
}
