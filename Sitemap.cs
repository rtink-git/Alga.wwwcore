using System.Xml;
using System.Xml.Linq;
using System.Collections.Concurrent;

namespace Alga.wwwcore;

class Sitemap
{
    static readonly XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
    static readonly XNamespace nsImage = "http://www.google.com/schemas/sitemap-image/1.1";
    static readonly XNamespace nsNews = "http://www.google.com/schemas/sitemap-news/0.9";

    public byte[] Build(ConcurrentDictionary<string, Models.Sitemap> list)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = System.Text.Encoding.UTF8,
            Async = false // синхронно быстрее
        };

        using var ms = new MemoryStream();
        using (var writer = XmlWriter.Create(ms, settings))
        {
            // bool hasNews = list.Any(i => i.News is not null);
            // bool hasImg = list.Any(i => i.Images is { Count: > 0 });

            writer.WriteStartDocument();
            writer.WriteStartElement("urlset", xmlns.NamespaceName);
            // if (hasImg) writer.WriteAttributeString("xmlns", "image", null, nsImage.NamespaceName);
            // if (hasNews) writer.WriteAttributeString("xmlns", "news", null, nsNews.NamespaceName);

            foreach (var i in list)
            {
                writer.WriteStartElement("url", xmlns.NamespaceName);
                writer.WriteElementString("loc", xmlns.NamespaceName, i.Key);

                if (i.Value.Lastmod is not null) //  && i.News is null
                    writer.WriteElementString("lastmod", xmlns.NamespaceName, i.Value.Lastmod.Value.ToString("yyyy-MM-dd"));

                // if (i.ChangeFreq is not null && i.News is null)
                //     writer.WriteElementString("changefreq", xmlns.NamespaceName, i.ChangeFreq.Value.ToString().ToLowerInvariant());

                // if (i.Priority is not null && i.News is null)
                //     writer.WriteElementString("priority", xmlns.NamespaceName,
                //         i.Priority.Value.ToString("G").AsSpan(1).ToString().Replace('_', '.'));

                // if (i.Images is { Count: > 0 })
                // {
                //     foreach (var img in i.Images)
                //     {
                //         writer.WriteStartElement("image", nsImage.NamespaceName);
                //         writer.WriteElementString("loc", nsImage.NamespaceName, img);
                //         writer.WriteEndElement();
                //     }
                // }

                // if (i.News is not null)
                // {
                //     writer.WriteStartElement("news", nsNews.NamespaceName);
                //     writer.WriteStartElement("publication", nsNews.NamespaceName);
                //     writer.WriteElementString("name", nsNews.NamespaceName, i.News.Value.PublicationName);
                //     writer.WriteElementString("language", nsNews.NamespaceName, i.News.Value.PublicationLanguageCode);
                //     writer.WriteEndElement();
                //     writer.WriteElementString("publication_date", nsNews.NamespaceName, i.LastMod?.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                //     writer.WriteElementString("title", nsNews.NamespaceName, i.News.Value.Title);
                //     writer.WriteEndElement();
                // }

                writer.WriteEndElement(); // </url>
            }

            writer.WriteEndElement(); // </urlset>
            writer.WriteEndDocument();
        }

        return ms.ToArray();
    }
}