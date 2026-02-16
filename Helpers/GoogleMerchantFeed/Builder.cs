using System.Xml;
using System.Text;
using System.Globalization;

namespace Alga.wwwcore.Helpers.GoogleMerchantFeed;

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
        Async = false,      // синхронный режим быстрее для MemoryStream
        NewLineChars = "\n",
        NewLineHandling = NewLineHandling.Replace
    };

    public Builder(string shopTitle, string shopLink, string shopDescription)
    {
        _shopTitle = shopTitle ?? throw new ArgumentNullException(nameof(shopTitle));
        _shopLink = shopLink ?? throw new ArgumentNullException(nameof(shopLink));
        _shopDescription = shopDescription ?? throw new ArgumentNullException(nameof(shopDescription));
    }

    public MemoryStream GenerateXmlStream(Req req)
    {
        var stream = new MemoryStream();
        using var w = XmlWriter.Create(stream, XmlSettings);

        // RSS start
        w.WriteStartElement("rss");
        w.WriteAttributeString("version", "2.0");
        w.WriteAttributeString("xmlns", "g", null, Ns);

        w.WriteStartElement("channel");
        w.WriteElementString("title", _shopTitle);
        w.WriteElementString("link", _shopLink);
        w.WriteElementString("description", _shopDescription);

        if (req.Items != null)
        {
            foreach (var item in req.Items)
            {
                w.WriteStartElement("item");

                w.WriteElementString("g", "id", Ns, item.Id);
                w.WriteElementString("g", "title", Ns, item.Title);
                w.WriteElementString("g", "link", Ns, item.Link);

                if (!string.IsNullOrWhiteSpace(item.Description))
                    w.WriteElementString("g", "description", Ns, item.Description);

                if (!string.IsNullOrWhiteSpace(item.ImageLink))
                    w.WriteElementString("g", "image_link", Ns, item.ImageLink);

                if (!string.IsNullOrWhiteSpace(item.AdditionalImageLink))
                    w.WriteElementString("g", "additional_image_link", Ns, item.AdditionalImageLink);

                // форматируем цену без лишних аллокаций
                var price = item.Price.ToString("F2", CultureInfo.InvariantCulture) + " " + item.Currency;
                w.WriteElementString("g", "price", Ns, price);

                w.WriteElementString("g", "availability", Ns, item.Availability);
                w.WriteElementString("g", "condition", Ns, item.Condition);

                if (!string.IsNullOrWhiteSpace(item.Brand)) w.WriteElementString("g", "brand", Ns, item.Brand);
                if (!string.IsNullOrWhiteSpace(item.Gtin)) w.WriteElementString("g", "gtin", Ns, item.Gtin);
                if (!string.IsNullOrWhiteSpace(item.Mpn)) w.WriteElementString("g", "mpn", Ns, item.Mpn);
                if (!string.IsNullOrWhiteSpace(item.IdentifierExists)) w.WriteElementString("g", "identifier_exists", Ns, item.IdentifierExists);
                if (!string.IsNullOrWhiteSpace(item.ProductType)) w.WriteElementString("g", "product_type", Ns, item.ProductType);
                if (!string.IsNullOrWhiteSpace(item.GoogleProductCategory)) w.WriteElementString("g", "google_product_category", Ns, item.GoogleProductCategory);

                if (item.CustomAttributes != null)
                {
                    foreach (var kv in item.CustomAttributes)
                        w.WriteElementString("g", kv.Key, Ns, kv.Value);
                }

                w.WriteEndElement(); // </item>
            }
        }

        w.WriteEndElement(); // </channel>
        w.WriteEndElement(); // </rss>

        w.Flush();
        stream.Position = 0;
        return stream;
    }

    public byte[] GenerateXmlInBytes(Req req)
    {
        using var ms = GenerateXmlStream(req);
        return ms.ToArray();
    }
}