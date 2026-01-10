namespace Alga.wwwcore.Helpers.GoogleMerchantFeed;

public sealed class ReqItem
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Link { get; init; }

    public string? Description { get; init; }
    public string? ImageLink { get; init; }
    public string? AdditionalImageLink { get; init; }

    public required decimal Price { get; init; } // в валюте магазина, например 2990.00
    public required string Currency { get; init; } // "RUB", "USD" и т.д.

    public string Availability { get; init; } = "in stock"; // in stock / out of stock / preorder
    public string Condition { get; init; } = "new"; // new / refurbished / used

    public string? Brand { get; init; }
    public string? Gtin { get; init; } // EAN, UPC, JAN
    public string? Mpn { get; init; }
    public string? IdentifierExists { get; init; } = "yes"; // если нет gtin/mpn

    public string? ProductType { get; init; }   // твоя категория
    public string? GoogleProductCategory { get; init; } // путь из классификатора Google

    // Можно добавить сколько угодно: shipping, tax, custom_label_0 и т.д.
    public Dictionary<string, string>? CustomAttributes { get; init; }
}