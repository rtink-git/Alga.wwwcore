using System.Globalization;
using System.Text;

namespace Alga.wwwcore.Helpers.YandexEventsFeed;

public sealed class Builder
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

    private static readonly Encoding Utf8 = Encoding.UTF8;

    public Builder(
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

    public byte[] GenerateXmlInBytes(Req req)
    {
        using var ms = new MemoryStream();
        var writer = ms;

        void WriteUtf8(string s)
        {
            var bytes = Utf8.GetBytes(s);
            writer.Write(bytes, 0, bytes.Length);
        }

        void WriteElement(string name, string? value)
        {
            if (string.IsNullOrEmpty(value)) return;
            WriteUtf8($"<{name}>{System.Net.WebUtility.HtmlEncode(value)}</{name}>");
        }

        // RSS start
        WriteUtf8($"<rss version=\"2.0\" xmlns:yandex=\"http://news.yandex.ru\" xmlns:media=\"http://search.yahoo.com/mrss/\">");
        WriteUtf8("<channel>");
        WriteElement("title", _feedTitle);
        WriteElement("link", _feedLink);
        WriteElement("description", _feedDescription);
        WriteElement("language", _language);
        WriteElement("ttl", _ttl.ToString());
        WriteElement("lastBuildDate", DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture));

        if (!string.IsNullOrEmpty(_company)) WriteElement("yandex:company", _company);
        if (!string.IsNullOrEmpty(_region)) WriteElement("yandex:region", _region);

        if (!string.IsNullOrWhiteSpace(_imageUrl))
        {
            WriteUtf8("<image>");
            WriteElement("url", _imageUrl);
            WriteElement("title", _imageTitle ?? _feedTitle);
            WriteElement("link", _imageLink ?? _feedLink);
            WriteUtf8("</image>");
        }

        foreach (var item in req.Items)
        {
            WriteUtf8("<item>");
            WriteElement("title", item.Title);
            WriteElement("link", item.Link);

            WriteUtf8($"<guid isPermaLink=\"true\">{System.Net.WebUtility.HtmlEncode(item.Link)}</guid>");
            WriteElement("pubDate", item.EventDateUtc.ToString("r", CultureInfo.InvariantCulture));
            WriteElement("category", item.EventType);
            WriteElement("description", item.Description);

            if (!string.IsNullOrWhiteSpace(item.FullText))
                WriteElement("yandex:full-text", item.FullText);

            if (!string.IsNullOrWhiteSpace(item.ImageUrl))
            {
                WriteUtf8($"<media:content url=\"{item.ImageUrl}\" type=\"{(string.IsNullOrWhiteSpace(item.ImageMimeType) ? "image/jpeg" : item.ImageMimeType)}\" />");
            }

            if (item.Price.HasValue)
                WriteElement("yandex:price", item.Price.Value.ToString("F2", CultureInfo.InvariantCulture));

            WriteElement("yandex:currency", item.Currency);

            WriteUtf8("</item>");
        }

        WriteUtf8("</channel></rss>");
        return ms.ToArray();
    }
}


// using System.Globalization;
// using System.Text;
// using System.Xml;

// namespace Alga.wwwcore.Helpers.YandexEventsFeed;

// /// <summary>
// /// Generates an event-based RSS feed for Yandex (RSS 2.0 + Yandex + Media).
// /// </summary>
// public sealed class Builder
// {
//     private readonly string _feedTitle;
//     private readonly string _feedLink;
//     private readonly string _feedDescription;
//     private readonly string _language;
//     private readonly int _ttl;
//     private readonly string? _company;
//     private readonly string? _region;
//     private readonly string? _imageUrl;
//     private readonly string? _imageTitle;
//     private readonly string? _imageLink;

//     private static readonly XmlWriterSettings XmlSettings = new()
//     {
//         Indent = true,
//         Encoding = Encoding.UTF8,
//         Async = true
//     };

//     public Builder(
//         string feedTitle,
//         string feedLink,
//         string feedDescription,
//         string language = "ru",
//         int ttl = 60,
//         string? company = null,
//         string? region = null,
//         string? imageUrl = null,
//         string? imageTitle = null,
//         string? imageLink = null)
//     {
//         _feedTitle = feedTitle ?? throw new ArgumentNullException(nameof(feedTitle));
//         _feedLink = feedLink ?? throw new ArgumentNullException(nameof(feedLink));
//         _feedDescription = feedDescription ?? throw new ArgumentNullException(nameof(feedDescription));
//         _language = language;
//         _ttl = ttl;
//         _company = company;
//         _region = region;
//         _imageUrl = imageUrl;
//         _imageTitle = imageTitle;
//         _imageLink = imageLink;
//     }

//     public async Task<MemoryStream> GenerateXmlStreamAsync(Req req)
//     {
//         using var stream = new MemoryStream();
//         await using var writer = XmlWriter.Create(stream, XmlSettings);

//         // <rss>
//         await writer.WriteStartElementAsync(null, "rss", null);
//         await writer.WriteAttributeStringAsync(null, "version", null, "2.0");
//         await writer.WriteAttributeStringAsync("xmlns", "yandex", null, "http://news.yandex.ru");
//         await writer.WriteAttributeStringAsync("xmlns", "media", null, "http://search.yahoo.com/mrss/");

//         // <channel>
//         await writer.WriteStartElementAsync(null, "channel", null);

//         await writer.WriteElementStringAsync(null, "title", null, _feedTitle);
//         await writer.WriteElementStringAsync(null, "link", null, _feedLink);
//         await writer.WriteElementStringAsync(null, "description", null, _feedDescription);
//         await writer.WriteElementStringAsync(null, "language", null, _language);
//         await writer.WriteElementStringAsync(null, "ttl", null, _ttl.ToString());
//         await writer.WriteElementStringAsync(null, "lastBuildDate", null, DateTime.UtcNow.ToString("r"));

//         if (_company != null) await writer.WriteElementStringAsync("yandex", "company", "http://news.yandex.ru", _company);
//         if (_region != null) await writer.WriteElementStringAsync("yandex", "region", "http://news.yandex.ru", _region);


//         // <image> — логотип канала
//         if (!string.IsNullOrWhiteSpace(_imageUrl))
//         {
//             await writer.WriteStartElementAsync(null, "image", null);

//             await writer.WriteElementStringAsync(null, "url", null, _imageUrl);
//             await writer.WriteElementStringAsync(null, "title", null, _imageTitle ?? _feedTitle);
//             await writer.WriteElementStringAsync(null, "link", null, _imageLink ?? _feedLink);

//             await writer.WriteEndElementAsync(); // </image>
//         }

//         foreach (var item in req.Items)
//             await WriteItemAsync(writer, item);

//         await writer.WriteEndElementAsync(); // </channel>
//         await writer.WriteEndElementAsync(); // </rss>

//         await writer.FlushAsync();
//         return stream;
//     }

//     public async Task<byte[]> GenerateXmlInBytesAsync(Req items)
//     {
//         await using var stream = await GenerateXmlStreamAsync(items);
//         return stream.ToArray();
//     }

//     private async Task WriteItemAsync(XmlWriter writer, ReqItem item)
//     {
//         await writer.WriteStartElementAsync(null, "item", null);

//         await writer.WriteElementStringAsync(null, "title", null, item.Title);
//         await writer.WriteElementStringAsync(null, "link", null, item.Link);

//         await writer.WriteStartElementAsync(null, "guid", null);
//         await writer.WriteAttributeStringAsync(null, "isPermaLink", null, "true");
//         await writer.WriteStringAsync(item.Link);
//         await writer.WriteEndElementAsync();

//         await writer.WriteElementStringAsync(
//             null,
//             "pubDate",
//             null,
//             item.EventDateUtc.ToString("r", CultureInfo.InvariantCulture));

//         if (!string.IsNullOrWhiteSpace(item.EventType))
//             await writer.WriteElementStringAsync(null, "category", null, item.EventType);

//         if (!string.IsNullOrWhiteSpace(item.Description))
//             await writer.WriteElementStringAsync(null, "description", null, item.Description);

//         if (!string.IsNullOrWhiteSpace(item.FullText))
//         {
//             await writer.WriteElementStringAsync(
//                 "yandex",
//                 "full-text",
//                 "http://news.yandex.ru",
//                 item.FullText);
//         }

//         if (!string.IsNullOrWhiteSpace(item.ImageUrl))
//         {
//             await writer.WriteStartElementAsync("media", "content", "http://search.yahoo.com/mrss/");
//             await writer.WriteAttributeStringAsync(null, "url", null, item.ImageUrl);
//             await writer.WriteAttributeStringAsync(
//                 null,
//                 "type",
//                 null,
//                 string.IsNullOrWhiteSpace(item.ImageMimeType)
//                     ? "image/jpeg"
//                     : item.ImageMimeType);
//             await writer.WriteEndElementAsync();
//         }

//         if (item.Price.HasValue)
//         {
//             await writer.WriteElementStringAsync(
//                 "yandex",
//                 "price",
//                 "http://news.yandex.ru",
//                 item.Price.Value.ToString("F2", CultureInfo.InvariantCulture));
//         }

//         if (!string.IsNullOrWhiteSpace(item.Currency))
//         {
//             await writer.WriteElementStringAsync(
//                 "yandex",
//                 "currency",
//                 "http://news.yandex.ru",
//                 item.Currency);
//         }

//         await writer.WriteEndElementAsync(); // </item>
//     }
// }