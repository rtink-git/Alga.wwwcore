namespace Alga.wwwcore.Helpers.YandexMarketLanguage;

public sealed class Req
{
    public required IEnumerable<ReqOffer> Offers { get; set; }
    public IEnumerable<ReqCategory>? Categories { get; set; }
    public IEnumerable<ReqCurrency>? Currencies { get; set; }
}