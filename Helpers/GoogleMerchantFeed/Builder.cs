using System.Xml;
using System.Text;


namespace Alga.wwwcore.Helpers.GoogleMerchantFeed;

/// <summary>
/// Google Merchant Center Feed (RSS 2.0)
/// A fluent builder class for generating a compliant Google Merchant Center product feed in RSS 2.0/XML format. It streamlines the creation of product entries with all required and recommended attributes (id, title, price, link, image_link, availability, etc.) for use in Google Shopping ads, free listings, and other Google commerce features.
/// https://support.google.com/merchants/answer/7052112
/// </summary>
public class Builder
{
    private const string Ns = "http://base.google.com/ns/1.0";
    private readonly string _shopTitle;
    private readonly string _shopLink;
    private readonly string _shopDescription;

    private static readonly XmlWriterSettings XmlSettings = new()
    {
        Indent = true,
        Encoding = Encoding.UTF8,
        Async = true
    };

    public Builder(string shopTitle, string shopLink, string shopDescription)
    {
        _shopTitle = shopTitle ?? throw new ArgumentNullException(nameof(shopTitle));
        _shopLink = shopLink ?? throw new ArgumentNullException(nameof(shopLink));
        _shopDescription = shopDescription ?? throw new ArgumentNullException(nameof(shopDescription));
    }

    public async Task<MemoryStream> GenerateXmlStreamAsync(Req req)
    {
        using var stream = new MemoryStream();
        await using var w = XmlWriter.Create(stream, XmlSettings);

        await w.WriteStartElementAsync(null, "rss", null);
        await w.WriteAttributeStringAsync(null, "version", null, "2.0");
        await w.WriteAttributeStringAsync("xmlns", "g", null, Ns);

        await w.WriteStartElementAsync(null, "channel", null);

        await w.WriteElementStringAsync(null, "title", null, _shopTitle);
        await w.WriteElementStringAsync(null, "link", null, _shopLink);
        await w.WriteElementStringAsync(null, "description", null, _shopDescription);

        if (req.Items != null)
            foreach (var item in req.Items)
            {
                await w.WriteStartElementAsync(null, "item", null);

                await w.WriteElementStringAsync("g", "id", Ns, item.Id);
                await w.WriteElementStringAsync("g", "title", Ns, item.Title);
                await w.WriteElementStringAsync("g", "link", Ns, item.Link);

                if (!string.IsNullOrWhiteSpace(item.Description))
                    await w.WriteElementStringAsync("g", "description", Ns, item.Description);

                if (!string.IsNullOrWhiteSpace(item.ImageLink))
                    await w.WriteElementStringAsync("g", "image_link", Ns, item.ImageLink);

                if (!string.IsNullOrWhiteSpace(item.AdditionalImageLink))
                    await w.WriteElementStringAsync("g", "additional_image_link", Ns, item.AdditionalImageLink);

                await w.WriteElementStringAsync("g", "price", Ns, $"{item.Price:F2} {item.Currency}".Replace(',', '.'));
                await w.WriteElementStringAsync("g", "availability", Ns, item.Availability);
                await w.WriteElementStringAsync("g", "condition", Ns, item.Condition);

                if (!string.IsNullOrWhiteSpace(item.Brand))
                    await w.WriteElementStringAsync("g", "brand", Ns, item.Brand);

                if (!string.IsNullOrWhiteSpace(item.Gtin))
                    await w.WriteElementStringAsync("g", "gtin", Ns, item.Gtin);

                if (!string.IsNullOrWhiteSpace(item.Mpn))
                    await w.WriteElementStringAsync("g", "mpn", Ns, item.Mpn);

                if (!string.IsNullOrWhiteSpace(item.IdentifierExists))
                    await w.WriteElementStringAsync("g", "identifier_exists", Ns, item.IdentifierExists);

                if (!string.IsNullOrWhiteSpace(item.ProductType))
                    await w.WriteElementStringAsync("g", "product_type", Ns, item.ProductType);

                if (!string.IsNullOrWhiteSpace(item.GoogleProductCategory))
                    await w.WriteElementStringAsync("g", "google_product_category", Ns, item.GoogleProductCategory);

                if (item.CustomAttributes != null)
                    foreach (var kv in item.CustomAttributes)
                        await w.WriteElementStringAsync("g", kv.Key, Ns, kv.Value);

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