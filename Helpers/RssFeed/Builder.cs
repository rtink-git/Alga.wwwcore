using System.Xml;
using System.Text;
using System.Globalization;

namespace Alga.wwwcore.Helpers.RssFeed;

public class Builder
{
    private readonly string _feedTitle;
    private readonly string _feedLink;
    private readonly string _feedDescription;

    private static readonly XmlWriterSettings XmlSettings = new()
    {
        Indent = true,
        Encoding = Encoding.UTF8,
        Async = true
    };

    public Builder(string feedTitle, string feedLink, string feedDescription)
    {
        _feedTitle = feedTitle ?? throw new ArgumentNullException(nameof(feedTitle));
        _feedLink = feedLink ?? throw new ArgumentNullException(nameof(feedLink));
        _feedDescription = feedDescription ?? throw new ArgumentNullException(nameof(feedDescription));
    }

    public async Task<MemoryStream> GenerateXmlStreamAsync(Req req)
    {
        using var stream = new MemoryStream();
        await using var w = XmlWriter.Create(stream, XmlSettings);

        await w.WriteStartElementAsync(null, "rss", null);
        await w.WriteAttributeStringAsync(null, "version", null, "2.0");

        await w.WriteStartElementAsync(null, "channel", null);

        await w.WriteElementStringAsync(null, "title", null, _feedTitle);
        await w.WriteElementStringAsync(null, "link", null, _feedLink);
        await w.WriteElementStringAsync(null, "description", null, _feedDescription);
        await w.WriteElementStringAsync(null, "lastBuildDate", null, DateTime.UtcNow.ToString("r")); // RFC822

        foreach (var item in req.ReqItems)
        {
            await w.WriteStartElementAsync(null, "item", null);

            await w.WriteElementStringAsync(null, "title", null, item.Title);
            await w.WriteElementStringAsync(null, "link", null, item.Link);

            if (!string.IsNullOrWhiteSpace(item.Description))
                await w.WriteElementStringAsync(null, "description", null, item.Description);

            if (!string.IsNullOrWhiteSpace(item.Author))
                await w.WriteElementStringAsync(null, "author", null, item.Author);

            if (!string.IsNullOrWhiteSpace(item.Guid))
            {
                await w.WriteStartElementAsync(null, "guid", null);
                if (item.GuidIsPermaLink.HasValue)
                    await w.WriteAttributeStringAsync(null, "isPermaLink", null, item.GuidIsPermaLink.Value ? "true" : "false");
                await w.WriteStringAsync(item.Guid);
                await w.WriteEndElementAsync();
            }

            if (item.PublicationDate.HasValue)
                await w.WriteElementStringAsync(null, "pubDate", null, item.PublicationDate.Value.ToString("r"));

            if (item.UpdatedDate.HasValue)
                await w.WriteElementStringAsync(null, "updated", null, item.UpdatedDate.Value.ToString("r"));

            // Категории
            if (item.Categories != null)
            {
                foreach (var cat in item.Categories)
                    await w.WriteElementStringAsync(null, "category", null, cat);
            }

            // Тип контента
            if (!string.IsNullOrWhiteSpace(item.Type))
                await w.WriteElementStringAsync(null, "category", null, item.Type);

            // Изображение/медиа
            if (!string.IsNullOrWhiteSpace(item.ImageUrl))
            {
                await w.WriteStartElementAsync(null, "enclosure", null);
                await w.WriteAttributeStringAsync(null, "url", null, item.ImageUrl);

                // Используем MIME из модели или по умолчанию image/jpeg
                var mimeType = string.IsNullOrWhiteSpace(item.ImageMimeType) ? "image/jpeg" : item.ImageMimeType;
                await w.WriteAttributeStringAsync(null, "type", null, mimeType);

                await w.WriteEndElementAsync();
            }

            if (item.Price.HasValue)
                await w.WriteElementStringAsync(null, "price", null, item.Price.Value.ToString("F2", CultureInfo.InvariantCulture));
            if (!string.IsNullOrWhiteSpace(item.Currency))
                await w.WriteElementStringAsync(null, "currency", null, item.Currency);

            if (item.CustomElements != null)
            {
                foreach (var kv in item.CustomElements)
                    await w.WriteElementStringAsync(null, kv.Key, null, kv.Value);
            }

            await w.WriteEndElementAsync(); // </item>
        }

        await w.WriteEndElementAsync(); // </channel>
        await w.WriteEndElementAsync(); // </rss>

        await w.FlushAsync();
        return stream;
    }

    public async Task<byte[]> GenerateXmlInBytesAsync(Req req)
    {
        await using var stream = await GenerateXmlStreamAsync(req);
        return stream.ToArray();
    }
}
