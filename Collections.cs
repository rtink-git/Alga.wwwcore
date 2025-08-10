using System.Collections.Concurrent;

namespace Alga.wwwcore;
public class Collections
{
    internal static readonly ConcurrentDictionary<string, Models.Sitemap> VisitedUrlsMap = new(concurrencyLevel: Environment.ProcessorCount, capacity: 100);
}
