using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Alga.wwwcore.Helpers.SitemapXML;

/// <summary>
/// What are Sitemaps? https://sitemaps.org/protocol.html
/// </summary>
public static class Builder
{
    private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
    private static readonly XNamespace ImageNs = "http://www.google.com/schemas/sitemap-image/1.1";
    private static readonly XNamespace VideoNs = "http://www.google.com/schemas/sitemap-video/1.1";
    private static readonly XNamespace ProductNs = "http://www.google.com/schemas/sitemap-products/1.1";
    private static readonly XNamespace NewsNs = "http://www.google.com/schemas/sitemap-news/0.9";
    private static readonly XNamespace XhtmlNs = "http://www.w3.org/1999/xhtml";

    public static string GenerateString(IEnumerable<Models.Url> urls)
    {
        var bytes = GenerateBytes(urls);
        return Encoding.UTF8.GetString(bytes);
    }

    public static string GenerateSitemapIndexString(IEnumerable<Models.IndexItem> items)
    {
        var bytes = GenerateSitemapIndexBytes(items);
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] GenerateBytes(IEnumerable<Models.Url> urls)
    {
        var urlset = BuildUrlSetElement(urls.Where(x => !string.IsNullOrWhiteSpace(x.Loc)));
        var doc = new XDocument(urlset);
        return SerializeToUtf8Bytes(doc);
    }

    public static byte[] GenerateSitemapIndexBytes(IEnumerable<Models.IndexItem> items)
    {
        var index = new XElement(Ns + "sitemapindex",
            items.Select(i =>
            {
                var el = new XElement(Ns + "sitemap",
                    new XElement(Ns + "loc", i.Loc)
                );
                if (i.LastMod.HasValue)
                    el.Add(new XElement(Ns + "lastmod", i.LastMod.Value.ToString("yyyy-MM-dd")));
                return el;
            })
        );

        var doc = new XDocument(index);
        return SerializeToUtf8Bytes(doc);
    }

    private static byte[] SerializeToUtf8Bytes(XDocument doc)
    {
        using var memoryStream = new MemoryStream();
        
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = false // XmlWriter сам добавит правильную декларацию
        };

        using var writer = XmlWriter.Create(memoryStream, settings);
        doc.WriteTo(writer);
        writer.Flush();

        return memoryStream.ToArray();
    }

    private static XElement BuildUrlSetElement(IEnumerable<Models.Url> urls)
    {
        var urlset = new XElement(Ns + "urlset",
            new XAttribute(XNamespace.Xmlns + "image", ImageNs),
            new XAttribute(XNamespace.Xmlns + "video", VideoNs),
            new XAttribute(XNamespace.Xmlns + "product", ProductNs),
            new XAttribute(XNamespace.Xmlns + "news", NewsNs),
            new XAttribute(XNamespace.Xmlns + "xhtml", XhtmlNs)
        );

        foreach (var u in urls)
        {
            var url = new XElement(Ns + "url",
                new XElement(Ns + "loc", u.Loc)
            );

            if (u.LastMod.HasValue)
                url.Add(new XElement(Ns + "lastmod", u.LastMod.Value.ToString("yyyy-MM-dd")));

            if (!string.IsNullOrWhiteSpace(u.ChangeFreq))
                url.Add(new XElement(Ns + "changefreq", u.ChangeFreq));

            if (u.Priority.HasValue)
                url.Add(new XElement(Ns + "priority", u.Priority.Value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)));

            // Альтернативные языки
            if (u.AlternateUrls?.Count > 0)
            {
                foreach (var alt in u.AlternateUrls)
                {
                    url.Add(new XElement(XhtmlNs + "link",
                        new XAttribute("rel", "alternate"),
                        new XAttribute("hreflang", alt.Key),
                        new XAttribute("href", alt.Value)
                    ));
                }
            }

            // Изображения
            if (u.Images?.Count > 0)
            {
                foreach (var img in u.Images)
                {
                    var imageEl = new XElement(ImageNs + "image",
                        new XElement(ImageNs + "loc", img.Url)
                    );
                    if (!string.IsNullOrWhiteSpace(img.Caption))
                        imageEl.Add(new XElement(ImageNs + "caption", img.Caption));
                    if (!string.IsNullOrWhiteSpace(img.Title))
                        imageEl.Add(new XElement(ImageNs + "title", img.Title));
                    if (!string.IsNullOrWhiteSpace(img.License))
                        imageEl.Add(new XElement(ImageNs + "license", img.License));

                    url.Add(imageEl);
                }
            }

            // Видео
            if (u.Video is not null)
            {
                var v = u.Video;
                var videoEl = new XElement(VideoNs + "video");
                if (!string.IsNullOrWhiteSpace(v.ThumbnailUrl))
                    videoEl.Add(new XElement(VideoNs + "thumbnail_loc", v.ThumbnailUrl));
                if (!string.IsNullOrWhiteSpace(v.Title))
                    videoEl.Add(new XElement(VideoNs + "title", v.Title));
                if (!string.IsNullOrWhiteSpace(v.Description))
                    videoEl.Add(new XElement(VideoNs + "description", v.Description));
                if (!string.IsNullOrWhiteSpace(v.ContentUrl))
                    videoEl.Add(new XElement(VideoNs + "content_loc", v.ContentUrl));
                if (v.PublicationDate.HasValue)
                    videoEl.Add(new XElement(VideoNs + "publication_date", v.PublicationDate.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")));
                if (v.DurationSeconds.HasValue)
                    videoEl.Add(new XElement(VideoNs + "duration", v.DurationSeconds.Value));
                if (!string.IsNullOrWhiteSpace(v.FamilyFriendly))
                    videoEl.Add(new XElement(VideoNs + "family_friendly", v.FamilyFriendly));

                url.Add(videoEl);
            }

            // Продукты
            if (u.Product is not null)
            {
                var p = u.Product;
                var productEl = new XElement(ProductNs + "product");
                if (!string.IsNullOrWhiteSpace(p.Id))
                    productEl.Add(new XElement(ProductNs + "id", p.Id));
                if (!string.IsNullOrWhiteSpace(p.Name))
                    productEl.Add(new XElement(ProductNs + "name", p.Name));
                if (!string.IsNullOrWhiteSpace(p.Description))
                    productEl.Add(new XElement(ProductNs + "description", p.Description));
                if (!string.IsNullOrWhiteSpace(p.Image))
                    productEl.Add(new XElement(ProductNs + "image", p.Image));
                if (!string.IsNullOrWhiteSpace(p.Brand))
                    productEl.Add(new XElement(ProductNs + "brand", p.Brand));
                if (!string.IsNullOrWhiteSpace(p.Category))
                    productEl.Add(new XElement(ProductNs + "category", p.Category));

                var price = new XElement(ProductNs + "price", p.Price.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture));
                if (!string.IsNullOrWhiteSpace(p.Currency))
                    price.SetAttributeValue("currency", p.Currency);
                productEl.Add(price);

                if (!string.IsNullOrWhiteSpace(p.Availability))
                    productEl.Add(new XElement(ProductNs + "availability", p.Availability));
                if (!string.IsNullOrWhiteSpace(p.Condition))
                    productEl.Add(new XElement(ProductNs + "condition", p.Condition));
                if (!string.IsNullOrWhiteSpace(p.Gtin))
                    productEl.Add(new XElement(ProductNs + "gtin", p.Gtin));

                url.Add(productEl);
            }

            // Новости
            if (u.News is not null)
            {
                var n = u.News;
                var newsEl = new XElement(NewsNs + "news",
                    new XElement(NewsNs + "publication",
                        new XElement(NewsNs + "name", n.PublicationName),
                        new XElement(NewsNs + "language", n.Language)
                    ),
                    new XElement(NewsNs + "title", n.Title),
                    n.PublicationDate.HasValue ? new XElement(NewsNs + "publication_date", n.PublicationDate.Value.ToString("yyyy-MM-dd")) : null
                );
                url.Add(newsEl);
            }

            urlset.Add(url);
        }

        return urlset;
    }
}


// using System.Text;
// using System.Xml;
// using System.Xml.Linq;

// namespace Alga.wwwcore.Helpers.SitemapXML;

// /// <summary>
// /// What are Sitemaps? https://sitemaps.org/protocol.html
// /// </summary>
// public static class Builder
// {
//     private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
//     private static readonly XNamespace ImageNs = "http://www.google.com/schemas/sitemap-image/1.1";
//     private static readonly XNamespace VideoNs = "http://www.google.com/schemas/sitemap-video/1.1";
//     private static readonly XNamespace ProductNs = "http://www.google.com/schemas/sitemap-products/1.1";
//     private static readonly XNamespace NewsNs = "http://www.google.com/schemas/sitemap-news/0.9";
//     private static readonly XNamespace XhtmlNs = "http://www.w3.org/1999/xhtml";

//     public static string Generate(IEnumerable<Models.Url> urls)
//     {
//         var urlset = new XElement(Ns + "urlset",
//             new XAttribute(XNamespace.Xmlns + "image", ImageNs),
//             new XAttribute(XNamespace.Xmlns + "video", VideoNs),
//             new XAttribute(XNamespace.Xmlns + "product", ProductNs),
//             new XAttribute(XNamespace.Xmlns + "news", NewsNs),
//             new XAttribute(XNamespace.Xmlns + "xhtml", XhtmlNs)
//         );

//         foreach (var u in urls.Where(x => !string.IsNullOrWhiteSpace(x.Loc)))
//         {
//             var url = new XElement(Ns + "url",
//                 new XElement(Ns + "loc", u.Loc)
//             );

//             if (u.LastMod.HasValue)
//                 url.Add(new XElement(Ns + "lastmod", u.LastMod.Value.ToString("yyyy-MM-dd")));

//             if (!string.IsNullOrWhiteSpace(u.ChangeFreq))
//                 url.Add(new XElement(Ns + "changefreq", u.ChangeFreq));

//             if (u.Priority.HasValue)
//                 url.Add(new XElement(Ns + "priority", u.Priority.Value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)));

//             // Альтернативные языки
//             if (u.AlternateUrls?.Count > 0)
//             {
//                 foreach (var alt in u.AlternateUrls)
//                 {
//                     url.Add(new XElement(XhtmlNs + "link",
//                         new XAttribute("rel", "alternate"),
//                         new XAttribute("hreflang", alt.Key),
//                         new XAttribute("href", alt.Value)
//                     ));
//                 }
//             }

//             // Изображения
//             if (u.Images?.Count > 0)
//             {
//                 foreach (var img in u.Images)
//                 {
//                     var imageEl = new XElement(ImageNs + "image",
//                         new XElement(ImageNs + "loc", img.Url)
//                     );
//                     if (!string.IsNullOrWhiteSpace(img.Caption))
//                         imageEl.Add(new XElement(ImageNs + "caption", img.Caption));
//                     if (!string.IsNullOrWhiteSpace(img.Title))
//                         imageEl.Add(new XElement(ImageNs + "title", img.Title));
//                     if (!string.IsNullOrWhiteSpace(img.License))
//                         imageEl.Add(new XElement(ImageNs + "license", img.License));

//                     url.Add(imageEl);
//                 }
//             }

//             // Видео
//             if (u.Video is not null)
//             {
//                 var v = u.Video;
//                 var videoEl = new XElement(VideoNs + "video");
//                 if (!string.IsNullOrWhiteSpace(v.ThumbnailUrl))
//                     videoEl.Add(new XElement(VideoNs + "thumbnail_loc", v.ThumbnailUrl));
//                 if (!string.IsNullOrWhiteSpace(v.Title))
//                     videoEl.Add(new XElement(VideoNs + "title", v.Title));
//                 if (!string.IsNullOrWhiteSpace(v.Description))
//                     videoEl.Add(new XElement(VideoNs + "description", v.Description));
//                 if (!string.IsNullOrWhiteSpace(v.ContentUrl))
//                     videoEl.Add(new XElement(VideoNs + "content_loc", v.ContentUrl));
//                 if (v.PublicationDate.HasValue)
//                     videoEl.Add(new XElement(VideoNs + "publication_date", v.PublicationDate.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")));
//                 if (v.DurationSeconds.HasValue)
//                     videoEl.Add(new XElement(VideoNs + "duration", v.DurationSeconds.Value));
//                 if (!string.IsNullOrWhiteSpace(v.FamilyFriendly))
//                     videoEl.Add(new XElement(VideoNs + "family_friendly", v.FamilyFriendly));

//                 url.Add(videoEl);
//             }

//             // Продукты
//             if (u.Product is not null)
//             {
//                 var p = u.Product;
//                 var productEl = new XElement(ProductNs + "product");
//                 if (!string.IsNullOrWhiteSpace(p.Id))
//                     productEl.Add(new XElement(ProductNs + "id", p.Id));
//                 if (!string.IsNullOrWhiteSpace(p.Name))
//                     productEl.Add(new XElement(ProductNs + "name", p.Name));
//                 if (!string.IsNullOrWhiteSpace(p.Description))
//                     productEl.Add(new XElement(ProductNs + "description", p.Description));
//                 if (!string.IsNullOrWhiteSpace(p.Image))
//                     productEl.Add(new XElement(ProductNs + "image", p.Image));
//                 if (!string.IsNullOrWhiteSpace(p.Brand))
//                     productEl.Add(new XElement(ProductNs + "brand", p.Brand));
//                 if (!string.IsNullOrWhiteSpace(p.Category))
//                     productEl.Add(new XElement(ProductNs + "category", p.Category));

//                 var price = new XElement(ProductNs + "price", p.Price.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture));
//                 if (!string.IsNullOrWhiteSpace(p.Currency))
//                     price.SetAttributeValue("currency", p.Currency);
//                 productEl.Add(price);

//                 if (!string.IsNullOrWhiteSpace(p.Availability))
//                     productEl.Add(new XElement(ProductNs + "availability", p.Availability));
//                 if (!string.IsNullOrWhiteSpace(p.Condition))
//                     productEl.Add(new XElement(ProductNs + "condition", p.Condition));
//                 if (!string.IsNullOrWhiteSpace(p.Gtin))
//                     productEl.Add(new XElement(ProductNs + "gtin", p.Gtin));

//                 url.Add(productEl);
//             }

//             // Новости
//             if (u.News is not null)
//             {
//                 var n = u.News;
//                 var newsEl = new XElement(NewsNs + "news",
//                     new XElement(NewsNs + "publication",
//                         new XElement(NewsNs + "name", n.PublicationName),
//                         new XElement(NewsNs + "language", n.Language)
//                     ),
//                     new XElement(NewsNs + "title", n.Title),
//                     n.PublicationDate.HasValue ? new XElement(NewsNs + "publication_date", n.PublicationDate.Value.ToString("yyyy-MM-dd")) : null
//                 );
//                 url.Add(newsEl);
//             }

//             urlset.Add(url);
//         }

//         // var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), urlset);

//         // using var sw = new StringWriter();
//         // using var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Encoding = System.Text.Encoding.UTF8 });
//         // doc.WriteTo(xw);
//         // xw.Flush();

//         var doc = new XDocument(urlset); // Без декларации

//         using var sw = new StringWriter();
//         // Принудительно пишем декларацию UTF-8
//         sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

//         using var xw = XmlWriter.Create(sw, new XmlWriterSettings 
//         { 
//             Indent = true,
//             OmitXmlDeclaration = true // Исключаем декларацию из XmlWriter
//         });

//         doc.WriteTo(xw);
//         xw.Flush();

//         string xmlString = sw.ToString();
//         return xmlString;
//     }

//     public static string GenerateSitemapIndex(IEnumerable<Models.IndexItem> items)
//     {
//         var index = new XElement(Ns + "sitemapindex",
//             items.Select(i =>
//             {
//                 var el = new XElement(Ns + "sitemap",
//                     new XElement(Ns + "loc", i.Loc)
//                 );
//                 if (i.LastMod.HasValue)
//                     el.Add(new XElement(Ns + "lastmod", i.LastMod.Value.ToString("yyyy-MM-dd")));
//                 return el;
//             })
//         );

//         // var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), index);

//         // using var sw = new StringWriter();
//         // using var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Encoding = System.Text.Encoding.UTF8 });
//         // doc.WriteTo(xw);
//         // xw.Flush();

//         // return sw.ToString();

//         var doc = new XDocument(index); // Без декларации

//         using var sw = new StringWriter();
//         // Принудительно пишем декларацию UTF-8
//         sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

//         using var xw = XmlWriter.Create(sw, new XmlWriterSettings 
//         { 
//             Indent = true,
//             OmitXmlDeclaration = true // Исключаем декларацию из XmlWriter
//         });

//         doc.WriteTo(xw);
//         xw.Flush();

//         string xmlString = sw.ToString();
//         return xmlString;
//     }
// }
