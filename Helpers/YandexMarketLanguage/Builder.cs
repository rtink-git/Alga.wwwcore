using System.Text;
using System.Xml;
using System.Globalization;
using System.Reflection.Metadata;

namespace Alga.wwwcore.Helpers.YandexMarketLanguage;

public class Builder
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

    public Builder(string shopName, string shopCompany, string shopUrl)
    {
        _shopName = shopName ?? throw new ArgumentNullException(nameof(shopName));
        _shopCompany = shopCompany ?? throw new ArgumentNullException(nameof(shopCompany));
        _shopUrl = shopUrl ?? throw new ArgumentNullException(nameof(shopUrl));
    }

    public async Task<byte[]> GenerateXmlAsync(
        IEnumerable<Models.OfferModel> offers,
        IEnumerable<Models.CategoryModel>? categories = null,
        IEnumerable<Models.CurrencyModel>? currencies = null)
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
            await w.WriteStartElementAsync(null, "currency", null);
            await w.WriteAttributeStringAsync(null, "id", null, "RUB");
            await w.WriteAttributeStringAsync(null, "rate", null, "1");
            await w.WriteEndElementAsync();
        }
        await w.WriteEndElementAsync(); // </currencies>

        // Categories (если есть категории)
        if (categories != null && categories.Any())
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

            // Добавляем новые поля
            if (!string.IsNullOrEmpty(o.Vendor))
                await w.WriteElementStringAsync(null, "vendor", null, o.Vendor);

            if (!string.IsNullOrEmpty(o.VendorCode))
                await w.WriteElementStringAsync(null, "vendorCode", null, o.VendorCode);

            if (!string.IsNullOrEmpty(o.Barcode))
                await w.WriteElementStringAsync(null, "barcode", null, o.Barcode);

            if (o.WarrantyMonths.HasValue)
                await w.WriteElementStringAsync(null, "warranty", null, $"{o.WarrantyMonths} месяцев");

            if (o.Weight.HasValue)
                await w.WriteElementStringAsync(null, "weight", null, o.Weight.Value.ToString("0.##", CultureInfo.InvariantCulture));

            if (o.Dimensions.HasValue)
                await w.WriteElementStringAsync(null, "dimensions", null, $"{o.Dimensions.Value.Length}x{o.Dimensions.Value.Width}x{o.Dimensions.Value.Height}");

            if (o.Delivery.HasValue)
                await w.WriteElementStringAsync(null, "delivery", null, o.Delivery.Value ? "true" : "false");

            if (o.Pickup.HasValue)
                await w.WriteElementStringAsync(null, "pickup", null, o.Pickup.Value ? "true" : "false");

            if (o.Store.HasValue)
                await w.WriteElementStringAsync(null, "store", null, o.Store.Value ? "true" : "false");

            // Custom params (если есть)
            if (o.Params != null && o.Params.Any())
            {
                foreach (var param in o.Params)
                {
                    await w.WriteStartElementAsync(null, "param", null);
                    await w.WriteAttributeStringAsync(null, "name", null, param.Key);
                    await w.WriteStringAsync(param.Value);
                    await w.WriteEndElementAsync();
                }
            }

            await w.WriteEndElementAsync(); // </offer>
        }
        await w.WriteEndElementAsync(); // </offers>

        await w.WriteEndElementAsync(); // </shop>
        await w.WriteEndElementAsync(); // </yml_catalog>
        await w.WriteEndDocumentAsync();
        await w.FlushAsync();

        return stream.ToArray();
    }
}
