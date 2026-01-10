namespace Alga.wwwcore.Helpers.RssFeed;

public sealed class ReqItem
{
    // Основные
    public required string Title { get; init; }
    public required string Link { get; init; }
    public string? Description { get; init; }
    public string? Author { get; init; } // email или имя автора

    // Идентификатор
    public string? Guid { get; init; }
    public bool? GuidIsPermaLink { get; init; } = true;

    // Даты
    public DateTime? PublicationDate { get; init; }
    public DateTime? UpdatedDate { get; init; }

    // Категории и тип
    public IEnumerable<string>? Categories { get; init; }
    public string? Type { get; init; }

    // Медиа
    public string? ImageUrl { get; init; }
    public string? ImageMimeType { get; init; } // MIME типа изображения

    // Товары
    public decimal? Price { get; init; }
    public string? Currency { get; init; }

    // Кастомные элементы
    public Dictionary<string, string>? CustomElements { get; init; }
}
