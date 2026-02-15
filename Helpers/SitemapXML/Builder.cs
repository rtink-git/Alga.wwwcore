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

    public static string GenerateString(ReqUrlEnumerable req)
    {
        var bytes = GenerateBytes(req);
        return Encoding.UTF8.GetString(bytes);
    }

    public static string GenerateSitemapIndexString(ReqIndexItemEnumerable req)
    {
        var bytes = GenerateSitemapIndexBytes(req);
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] GenerateBytes(ReqUrlEnumerable req)
    {
        var urlset = BuildUrlSetElement(new ReqUrlEnumerable() { Urls = req.Urls.Where(x => !string.IsNullOrWhiteSpace(x.Loc)) });
        var doc = new XDocument(urlset);
        return SerializeToUtf8Bytes(doc);
    }

    public static byte[] GenerateSitemapIndexBytes(ReqIndexItemEnumerable req)
    {
        var index = new XElement(Ns + "sitemapindex",
            req.IndexItems.Select(i =>
            {
                var el = new XElement(Ns + "sitemap",
                    new XElement(Ns + "loc", i.Loc)
                );
                if (i.LastMod.HasValue)
                    el.Add(new XElement(Ns + "lastmod", i.LastMod.Value.ToString("yyyy-MM-ddTHH:mm:sszzz"))); //.ToString("yyyy-MM-dd")
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

    private static XElement BuildUrlSetElement(ReqUrlEnumerable req)
    {
        var urlset = new XElement(Ns + "urlset",
            new XAttribute(XNamespace.Xmlns + "image", ImageNs),
            new XAttribute(XNamespace.Xmlns + "video", VideoNs),
            new XAttribute(XNamespace.Xmlns + "product", ProductNs),
            new XAttribute(XNamespace.Xmlns + "news", NewsNs),
            new XAttribute(XNamespace.Xmlns + "xhtml", XhtmlNs)
        );

        foreach (var u in req.Urls)
        {
            var url = new XElement(Ns + "url",
                new XElement(Ns + "loc", u.Loc)
            );

            if (u.LastMod.HasValue)
                url.Add(new XElement(Ns + "lastmod", u.LastMod.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")));

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
                    n.PublicationDate.HasValue ? new XElement(NewsNs + "publication_date", n.PublicationDate.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")) : null
                );
                url.Add(newsEl);
            }

            urlset.Add(url);
        }

        return urlset;
    }
}