using System.Text;
using System.Xml;
using System.Globalization;

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
        Async = false // Асинхронность отключена
    };

    public Builder(string shopName, string shopCompany, string shopUrl)
    {
        _shopName = shopName ?? throw new ArgumentNullException(nameof(shopName));
        _shopCompany = shopCompany ?? throw new ArgumentNullException(nameof(shopCompany));
        _shopUrl = shopUrl ?? throw new ArgumentNullException(nameof(shopUrl));
    }

    public MemoryStream GenerateXmlStream(Req req)
    {
        var stream = new MemoryStream();
        using var w = XmlWriter.Create(stream, XmlSettings);

        w.WriteStartDocument();
        w.WriteStartElement("yml_catalog");
        w.WriteAttributeString("date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));

        w.WriteStartElement("shop");

        w.WriteElementString("name", _shopName);
        w.WriteElementString("company", _shopCompany);
        w.WriteElementString("url", _shopUrl);

        // Currencies (если переданы валюты)
        w.WriteStartElement("currencies");
        if (req.Currencies != null)
        {
            foreach (var c in req.Currencies)
            {
                w.WriteStartElement("currency");
                w.WriteAttributeString("id", c.Id);
                w.WriteAttributeString("rate", c.Rate.ToString("0.##", CultureInfo.InvariantCulture));
                w.WriteEndElement();
            }
        }
        else
        {
            w.WriteStartElement("currency");
            w.WriteAttributeString("id", "RUB");
            w.WriteAttributeString("rate", "1");
            w.WriteEndElement();
        }
        w.WriteEndElement(); // </currencies>

        // Categories (если есть категории)
        if (req.Categories != null && req.Categories.Any())
        {
            w.WriteStartElement("categories");
            foreach (var c in req.Categories)
            {
                w.WriteStartElement("category");
                w.WriteAttributeString("id", c.Id.ToString());
                if (c.ParentId.HasValue)
                    w.WriteAttributeString("parentId", c.ParentId.Value.ToString());
                w.WriteString(c.Name);
                w.WriteEndElement();
            }
            w.WriteEndElement(); // </categories>
        }

        // Offers (обязательная часть, всегда будет)
        w.WriteStartElement("offers");
        foreach (var o in req.Offers)
        {
            w.WriteStartElement("offer");
            w.WriteAttributeString("id", o.Id.ToString());
            w.WriteAttributeString("available", o.Available ? "true" : "false");

            w.WriteElementString("url", o.Url);
            w.WriteElementString("price", o.Price.ToString("0.##", CultureInfo.InvariantCulture));
            w.WriteElementString("currencyId", o.CurrencyId);
            w.WriteElementString("name", o.Name);

            if (!string.IsNullOrEmpty(o.Description))
                w.WriteElementString("description", o.Description);

            if (!string.IsNullOrEmpty(o.Picture))
                w.WriteElementString("picture", o.Picture);

            if (o.AdditionalPictures != null)
                foreach (var pic in o.AdditionalPictures)
                    w.WriteElementString("picture", pic);

            // Добавляем новые поля
            if (!string.IsNullOrEmpty(o.Vendor))
                w.WriteElementString("vendor", o.Vendor);

            if (!string.IsNullOrEmpty(o.VendorCode))
                w.WriteElementString("vendorCode", o.VendorCode);

            if (!string.IsNullOrEmpty(o.Barcode))
                w.WriteElementString("barcode", o.Barcode);

            if (o.WarrantyMonths.HasValue)
                w.WriteElementString("warranty", $"{o.WarrantyMonths} месяцев");

            if (o.Weight.HasValue)
                w.WriteElementString("weight", o.Weight.Value.ToString("0.##", CultureInfo.InvariantCulture));

            if (o.Dimensions.HasValue)
                w.WriteElementString("dimensions", $"{o.Dimensions.Value.Length}x{o.Dimensions.Value.Width}x{o.Dimensions.Value.Height}");

            if (o.Delivery.HasValue)
                w.WriteElementString("delivery", o.Delivery.Value ? "true" : "false");

            if (o.Pickup.HasValue)
                w.WriteElementString("pickup", o.Pickup.Value ? "true" : "false");

            if (o.Store.HasValue)
                w.WriteElementString("store", o.Store.Value ? "true" : "false");

            // Custom params (если есть)
            if (o.Params != null && o.Params.Any())
            {
                foreach (var param in o.Params)
                {
                    w.WriteStartElement("param");
                    w.WriteAttributeString("name", param.Key);
                    w.WriteString(param.Value);
                    w.WriteEndElement();
                }
            }

            w.WriteEndElement(); // </offer>
        }
        w.WriteEndElement(); // </offers>

        w.WriteEndElement(); // </shop>
        w.WriteEndElement(); // </yml_catalog>
        w.WriteEndDocument();
        w.Flush();

        return stream; // Возвращаем MemoryStream, который можно сразу передать клиенту
    }

    public byte[] GenerateXmlInBytes(Req req)
    {
        using var stream = GenerateXmlStream(req);
        return stream.ToArray();
    }
}