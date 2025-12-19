namespace Alga.wwwcore.Helpers.YandexMarketLanguage.Models;

public sealed class CategoryModel
{
    /// <summary>Unique category ID</summary>
    public required long Id { get; init; }

    /// <summary>Parent category ID (null for root categories)</summary>
    public long? ParentId { get; init; }

    /// <summary>Category name</summary>
    public required string Name { get; init; }
}