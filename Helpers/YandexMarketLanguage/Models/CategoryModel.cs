namespace Alga.wwwcore.Helpers.YandexMarketLanguage.Models;

public sealed class CategoryModel
{
    /// <summary>Unique category ID</summary>
    public required Guid Id { get; init; }

    /// <summary>Parent category ID (null for root categories)</summary>
    public Guid? ParentId { get; init; }

    /// <summary>Category name</summary>
    public required string Name { get; init; }
}