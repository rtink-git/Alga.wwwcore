namespace Alga.wwwcore.Helpers.YandexMarketLanguage.Models;

public sealed class CurrencyModel
{
    /// <summary>Currency code (RUB, USD, EUR)</summary>
    public required string Id { get; init; }

    /// <summary>Exchange rate (1, CBRF, NBU, +2%)</summary>
    public required decimal Rate { get; init; }
}
