using System;
using System.Collections.Generic;

namespace Alga.wwwcore.Helpers.YandexEventsFeed;

/// <summary>
/// Represents a real site event (not a static page).
/// </summary>
public sealed class Model
{
    /// <summary>
    /// Event title (must describe the actual event).
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Canonical page URL related to the event.
    /// </summary>
    public required string Link { get; init; }

    /// <summary>
    /// Event date in UTC (used for pubDate).
    /// </summary>
    public required DateTime EventDateUtc { get; init; }

    /// <summary>
    /// Event type/category (new-arrival, sale, price-change, etc.).
    /// </summary>
    public string? EventType { get; init; }

    /// <summary>
    /// Short description used as an announcement/snippet.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Full event text for Yandex indexing.
    /// </summary>
    public string? FullText { get; init; }

    /// <summary>
    /// Image URL associated with the event.
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Image MIME type (e.g. image/webp, image/jpeg).
    /// </summary>
    public string? ImageMimeType { get; init; }

    /// <summary>
    /// Optional price related to the event.
    /// </summary>
    public decimal? Price { get; init; }

    /// <summary>
    /// Currency code (e.g. RUB, USD).
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// Custom elements that can be added to the feed (e.g., author, guid).
    /// </summary>
    public Dictionary<string, string>? CustomElements { get; init; }
}
