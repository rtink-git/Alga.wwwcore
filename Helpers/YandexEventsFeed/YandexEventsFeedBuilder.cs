using System.Globalization;
using System.Text;
using System.Xml;

namespace Alga.wwwcore.Helpers.YandexEventsFeed;

/// <summary>
/// Generates an event-based RSS feed for Yandex (RSS 2.0 + Yandex + Media).
/// </summary>
public sealed class YandexEventsFeedBuilder
{
    private readonly string _feedTitle;
    private readonly string _feedLink;
    private readonly string _feedDescription;
    private readonly string _language;
    private readonly int _ttl;
    private readonly string? _company;
    private readonly string? _region;
    private readonly string? _imageUrl;
    private readonly string? _imageTitle;
    private readonly string? _imageLink;

    private static readonly XmlWriterSettings XmlSettings = new()
    {
        Indent = true,
        Encoding = Encoding.UTF8,
        Async = true
    };

    public YandexEventsFeedBuilder(
        string feedTitle,
        string feedLink,
        string feedDescription,
        string language = "ru",
        int ttl = 60,
        string? company = null,
        string? region = null,
        string? imageUrl = null,
        string? imageTitle = null,
        string? imageLink = null)
    {
        _feedTitle = feedTitle ?? throw new ArgumentNullException(nameof(feedTitle));
        _feedLink = feedLink ?? throw new ArgumentNullException(nameof(feedLink));
        _feedDescription = feedDescription ?? throw new ArgumentNullException(nameof(feedDescription));
        _language = language;
        _ttl = ttl;
        _company = company;
        _region = region;
        _imageUrl = imageUrl;
        _imageTitle = imageTitle;
        _imageLink = imageLink;
    }

    public async Task<byte[]> GenerateEventsFeedAsync(IEnumerable<Model> items)
    {
        using var stream = new MemoryStream();
        await using var writer = XmlWriter.Create(stream, XmlSettings);

        // <rss>
        await writer.WriteStartElementAsync(null, "rss", null);
        await writer.WriteAttributeStringAsync(null, "version", null, "2.0");
        await writer.WriteAttributeStringAsync("xmlns", "yandex", null, "http://news.yandex.ru");
        await writer.WriteAttributeStringAsync("xmlns", "media", null, "http://search.yahoo.com/mrss/");

        // <channel>
        await writer.WriteStartElementAsync(null, "channel", null);

        await writer.WriteElementStringAsync(null, "title", null, _feedTitle);
        await writer.WriteElementStringAsync(null, "link", null, _feedLink);
        await writer.WriteElementStringAsync(null, "description", null, _feedDescription);
        await writer.WriteElementStringAsync(null, "language", null, _language);
        await writer.WriteElementStringAsync(null, "ttl", null, _ttl.ToString());
        await writer.WriteElementStringAsync(null, "lastBuildDate", null, DateTime.UtcNow.ToString("r"));

        if (_company != null) await writer.WriteElementStringAsync("yandex", "company", "http://news.yandex.ru", _company);
        if (_region != null) await writer.WriteElementStringAsync("yandex", "region", "http://news.yandex.ru", _region);


        // <image> — логотип канала
        if (!string.IsNullOrWhiteSpace(_imageUrl))
        {
            await writer.WriteStartElementAsync(null, "image", null);

            await writer.WriteElementStringAsync(null, "url", null, _imageUrl);
            await writer.WriteElementStringAsync(null, "title", null, _imageTitle ?? _feedTitle);
            await writer.WriteElementStringAsync(null, "link", null, _imageLink ?? _feedLink);

            await writer.WriteEndElementAsync(); // </image>
        }

        foreach (var item in items)
        {
            await WriteItemAsync(writer, item);
        }

        await writer.WriteEndElementAsync(); // </channel>
        await writer.WriteEndElementAsync(); // </rss>

        await writer.FlushAsync();
        return stream.ToArray();
    }

    private static async Task WriteItemAsync(XmlWriter writer, Model item)
    {
        await writer.WriteStartElementAsync(null, "item", null);

        await writer.WriteElementStringAsync(null, "title", null, item.Title);
        await writer.WriteElementStringAsync(null, "link", null, item.Link);

        await writer.WriteStartElementAsync(null, "guid", null);
        await writer.WriteAttributeStringAsync(null, "isPermaLink", null, "true");
        await writer.WriteStringAsync(item.Link);
        await writer.WriteEndElementAsync();

        await writer.WriteElementStringAsync(
            null,
            "pubDate",
            null,
            item.EventDateUtc.ToString("r", CultureInfo.InvariantCulture));

        if (!string.IsNullOrWhiteSpace(item.EventType))
            await writer.WriteElementStringAsync(null, "category", null, item.EventType);

        if (!string.IsNullOrWhiteSpace(item.Description))
            await writer.WriteElementStringAsync(null, "description", null, item.Description);

        if (!string.IsNullOrWhiteSpace(item.FullText))
        {
            await writer.WriteElementStringAsync(
                "yandex",
                "full-text",
                "http://news.yandex.ru",
                item.FullText);
        }

        if (!string.IsNullOrWhiteSpace(item.ImageUrl))
        {
            await writer.WriteStartElementAsync("media", "content", "http://search.yahoo.com/mrss/");
            await writer.WriteAttributeStringAsync(null, "url", null, item.ImageUrl);
            await writer.WriteAttributeStringAsync(
                null,
                "type",
                null,
                string.IsNullOrWhiteSpace(item.ImageMimeType)
                    ? "image/jpeg"
                    : item.ImageMimeType);
            await writer.WriteEndElementAsync();
        }

        if (item.Price.HasValue)
        {
            await writer.WriteElementStringAsync(
                "yandex",
                "price",
                "http://news.yandex.ru",
                item.Price.Value.ToString("F2", CultureInfo.InvariantCulture));
        }

        if (!string.IsNullOrWhiteSpace(item.Currency))
        {
            await writer.WriteElementStringAsync(
                "yandex",
                "currency",
                "http://news.yandex.ru",
                item.Currency);
        }

        await writer.WriteEndElementAsync(); // </item>
    }
}