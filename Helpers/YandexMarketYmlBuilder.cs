using System.Text;
using System.Xml;
using System.Globalization;


namespace Alga.wwwcore.Helpers;

/// <summary>
/// Yandex Market YML Feed
/// A fluent builder class for generating a compliant Yandex Market YML product feed. Supports shop info, categories, currencies, and product offers.
/// https://yandex.ru/support/market/ru/
/// </summary>
public class YandexMarketYmlBuilder
{
    private readonly string _shopName;
    private readonly string _shopCompany;
    private readonly string _shopUrl;

    private static readonly XmlWriterSettings XmlSettings = new()
    {
        Indent = true,
        Encoding = Encoding.UTF8,
        Async = true
    };

    public YandexMarketYmlBuilder(string shopName, string shopCompany, string shopUrl)
    {
        _shopName = shopName ?? throw new ArgumentNullException(nameof(shopName));
        _shopCompany = shopCompany ?? throw new ArgumentNullException(nameof(shopCompany));
        _shopUrl = shopUrl ?? throw new ArgumentNullException(nameof(shopUrl));
    }

    public async Task<byte[]> GenerateXmlAsync(
        IEnumerable<OfferModel> offers,
        IEnumerable<CategoryModel>? categories = null,
        IEnumerable<CurrencyModel>? currencies = null)
    {
        using var stream = new MemoryStream();
        await using var w = XmlWriter.Create(stream, XmlSettings);

        await w.WriteStartDocumentAsync();
        await w.WriteStartElementAsync(null, "yml_catalog", null);
        await w.WriteAttributeStringAsync(null, "date", null, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));

        await w.WriteStartElementAsync(null, "shop", null);

        await w.WriteElementStringAsync(null, "name", null, _shopName);
        await w.WriteElementStringAsync(null, "company", null, _shopCompany);
        await w.WriteElementStringAsync(null, "url", null, _shopUrl);

        // Currencies (если переданы валюты)
        await w.WriteStartElementAsync(null, "currencies", null);
        if (currencies != null)
        {
            foreach (var c in currencies)
            {
                await w.WriteStartElementAsync(null, "currency", null);
                await w.WriteAttributeStringAsync(null, "id", null, c.Id);
                await w.WriteAttributeStringAsync(null, "rate", null, c.Rate.ToString("0.##", CultureInfo.InvariantCulture));
                await w.WriteEndElementAsync();
            }
        }
        else
        {
            // По умолчанию RUB
            await w.WriteStartElementAsync(null, "currency", null);
            await w.WriteAttributeStringAsync(null, "id", null, "RUB");
            await w.WriteAttributeStringAsync(null, "rate", null, "1");
            await w.WriteEndElementAsync();
        }
        await w.WriteEndElementAsync(); // </currencies>

        // Categories (если есть категории)
        if (categories != null && categories.Any())  // Проверка на null и пустой список
        {
            await w.WriteStartElementAsync(null, "categories", null);
            foreach (var c in categories)
            {
                await w.WriteStartElementAsync(null, "category", null);
                await w.WriteAttributeStringAsync(null, "id", null, c.Id.ToString());
                if (c.ParentId.HasValue)
                    await w.WriteAttributeStringAsync(null, "parentId", null, c.ParentId.Value.ToString());
                await w.WriteStringAsync(c.Name);
                await w.WriteEndElementAsync();
            }
            await w.WriteEndElementAsync(); // </categories>
        }

        // Offers (обязательная часть, всегда будет)
        await w.WriteStartElementAsync(null, "offers", null);
        foreach (var o in offers)
        {
            await w.WriteStartElementAsync(null, "offer", null);
            await w.WriteAttributeStringAsync(null, "id", null, o.Id.ToString());
            await w.WriteAttributeStringAsync(null, "available", null, o.Available ? "true" : "false");

            await w.WriteElementStringAsync(null, "url", null, o.Url);
            await w.WriteElementStringAsync(null, "price", null, o.Price.ToString("0.##", CultureInfo.InvariantCulture));
            await w.WriteElementStringAsync(null, "currencyId", null, o.CurrencyId);
            await w.WriteElementStringAsync(null, "categoryId", null, o.CategoryId.ToString());
            await w.WriteElementStringAsync(null, "name", null, o.Name);

            if (!string.IsNullOrEmpty(o.Description))
                await w.WriteElementStringAsync(null, "description", null, o.Description);

            if (!string.IsNullOrEmpty(o.Picture))
                await w.WriteElementStringAsync(null, "picture", null, o.Picture);

            if (o.AdditionalPictures != null)
                foreach (var pic in o.AdditionalPictures)
                    await w.WriteElementStringAsync(null, "picture", null, pic);

            await w.WriteEndElementAsync(); // </offer>
        }
        await w.WriteEndElementAsync(); // </offers>

        await w.WriteEndElementAsync(); // </shop>
        await w.WriteEndElementAsync(); // </yml_catalog>
        await w.WriteEndDocumentAsync();
        await w.FlushAsync();

        return stream.ToArray();
    }

    public sealed class CategoryModel
    {
        public required long Id { get; init; }
        public long? ParentId { get; init; }
        public required string Name { get; init; }
    }

    public sealed class OfferModel
    {
        public required long Id { get; init; }
        public required string Url { get; init; }
        public required decimal Price { get; init; }
        public string CurrencyId { get; init; } = "RUB";
        public required long CategoryId { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public string? Picture { get; init; }
        public List<string>? AdditionalPictures { get; init; }
        public bool Available { get; init; } = true;
    }

    public sealed class CurrencyModel
    {
        public required string Id { get; init; } // RUB, USD, EUR...
        public required decimal Rate { get; init; } // 1, 60, 0.85...
    }
}
