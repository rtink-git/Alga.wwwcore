namespace Alga.wwwcore.Helpers.YandexMarketLanguage;

public sealed class ReqOffer
{
    /// <summary>Unique offer ID</summary>
    public required Guid Id { get; init; }

    /// <summary>Offer availability</summary>
    public bool Available { get; init; } = true;

    /// <summary>Product page URL</summary>
    public required string Url { get; init; }

    /// <summary>Product price</summary>
    public required decimal Price { get; init; }

    /// <summary>Currency code (RUB, USD)</summary>
    public string CurrencyId { get; init; } = "RUB";

    /// <summary>Category ID</summary>
    public required Guid? CategoryId { get; init; }

    /// <summary>Product name</summary>
    public required string Name { get; init; }

    /// <summary>Short description (HTML allowed)</summary>
    public string? Description { get; init; }

    /// <summary>Main product image</summary>
    public string? Picture { get; init; }

    /// <summary>Additional images</summary>
    public List<string>? AdditionalPictures { get; init; }

    // -------------------
    // VERY IMPORTANT FOR MARKET
    // -------------------

    /// <summary>Vendor / brand</summary>
    public string? Vendor { get; init; }

    /// <summary>Manufacturer country</summary>
    public string? CountryOfOrigin { get; init; }

    /// <summary>SKU / article number</summary>
    public string? VendorCode { get; init; }

    /// <summary>Barcode (EAN / GTIN)</summary>
    public string? Barcode { get; init; }

    /// <summary>Warranty in months</summary>
    public int? WarrantyMonths { get; init; }

    /// <summary>Weight in kilograms</summary>
    public decimal? Weight { get; init; }

    /// <summary>Dimensions in cm (LxWxH)</summary>
    public (decimal Length, decimal Width, decimal Height)? Dimensions { get; init; }

    /// <summary>Is delivery available</summary>
    public bool? Delivery { get; init; }

    /// <summary>Is pickup available</summary>
    public bool? Pickup { get; init; }

    /// <summary>Is store pickup available</summary>
    public bool? Store { get; init; }

    /// <summary>Custom product attributes</summary>
    public Dictionary<string, string>? Params { get; init; }
}
