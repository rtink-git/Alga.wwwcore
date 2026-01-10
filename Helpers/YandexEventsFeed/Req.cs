using System;

namespace Alga.wwwcore.Helpers.YandexEventsFeed;

public class Req
{
    public required IEnumerable<ReqItem> Items { get; set; }
}
