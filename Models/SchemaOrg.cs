namespace Alga.wwwcore.Models;

/// <summary>
/// https://schema.org/docs/full.html?utm_source=chatgpt.com
/// https://developers.google.com/search/docs/appearance/structured-data/search-gallery?hl=ru
/// Google Structured Data Testing Tool: https://search.google.com/test/rich-results
/// </summary>
public sealed class SchemaOrg
{
    public required string Type { get; set; } // https://schema.org type
    public string? Id { get; set; }
    public string? Url { get; set; } // https://schema.org/url
    public string? Name { get; set; } // https://schema.org/name
    public string? CurrenciesAccepted { get; set; } // https://schema.org/currenciesAccepted
    public string? Description { get; set; } // https://schema.org/description
    public string? DownloadUrl { get; set; } // https://schema.org/downloadUrl
    public string? Email { get; set; } // https://schema.org/email
    public string? FoundingDate { get; set; } // https://schema.org/foundingDate
    public string? Headline { get; set; } // https://schema.org/headline
    public string? InLanguage { get; set; } // https://schema.org/inLanguage
    public string? InstallUrl { get; set; } // https://schema.org/installUrl
    public string? LegalName { get; set; } // https://schema.org/legalNamef
    public string? OperatingSystem { get; set; } // https://schema.org/operatingSystem
    public string? PaymentAccepted { get; set; } // https://schema.org/paymentAccepted
    public string? PriceRange { get; set; }
    public string? Sku { get; set; }
    public string? Telephone { get; set; } // https://schema.org/telephone
    public bool? IsAccessibleForFree { get; set; }
    public int? NumberOfItems { get; set; }
    public ContactPoint[]? ContactPoint { get; init; }
    public DateTime? DatePublished { get; set; } // https://schema.org/datePublished
    public DateTime? DateModified { get; set; } // https://schema.org/dateModified
    public DateTime? StartDate { get; set; } // https://schema.org/startDate
    public DateTime? EndDate { get; set; } // https://schema.org/endDate
    public Author? Author { get; set; }
    public BranchOf? BranchOf { get; set; }
    public PostalAddress? Address { get; set; }
    public ImageObject? Logo { get; set; }
    public SchemaOrg? MainEntity { get; set; }
    public SchemaOrg? SiteNavigation { get; set; }
    public string? MainEntityOfPage { get; set; } //https://schema.org/mainEntityOfPage
    public Offers? Offers { get; set; }
    public PlaceModel? AreaServed { get; set; }
    public Publisher? Publisher { get; set; }
    public string[]? ApplicationCategory { get; set; } // https://schema.org/applicationCategory
    public string? HasMap { get; set; } // https://schema.org/hasMap
    public string[]? OpeningHours { get; set; } // https://schema.org/openingHours
    public string[]? SameAs { get; set; } // https://schema.org/sameAs
    public string[]? Screenshot { get; set; } // https://schema.org/screenshot
    public SchemaOrg[]? IsPartOfs { get; set; }
    public GeoCordinates? GeoCordinates { get; set; }
    public PotentialAction[]? PotentialActions { get; set; }
    public ImageObject[]? Images { get; set; }
    public List<ItemListElement>? ItemListElements { get; set; }
}

/// <summary>
/// https://schema.org/address - Thing > Property :: address :: PostalAddress
/// </summary>
public sealed class PostalAddress
{
    public string? AddressCountry { get; set; }
    public string? AddressRegion { get; set; }
    public string? AddressLocality { get; set; }
    public string? StreetAddress { get; set; }
    public string? PostalCode { get; set; }
}

/// <summary>
/// https://schema.org/isPartOf
/// </summary>
public sealed class Author
{
    public required string Type { get; set; }
    public required string Name { get; set; }
    public string? Url { get; set; }
}

public sealed class BranchOf
{
    public string? Id { get; init; }
}

/// <summary>
/// https://schema.org/contactPoint
/// </summary>
public sealed class ContactPoint
{
    public required string Type { get; set; }
    public required string Telephone { get; set; }
    public required string ContactType { get; set; }
    public PlaceModel? AreaServed { get; set; }
    public string? AvailableLanguage { get; set; }
}

/// <summary>
/// https://schema.org/geo - Thing > Property :: geo :: GeoCoordinates
/// </summary>
public sealed class GeoCordinates
{
    public required double latitude { get; set; }
    public required double longitude { get; set; }
}

/// <summary>
/// https://schema.org/image
/// </summary>
// public sealed class Image
// {
//     public required string Type { get; set; } = "ImageObject";
//     public required string Url { get; set; }
//     public int? Width { get; set; }
//     public int? Height { get; set; }
//     public string? Caption { get; set; }
// }

public sealed class ImageObject
{
    public string? Url { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? Caption { get; set; }
    public string? EncodingFormat { get; set; }
    public bool? RepresentativeOfPage { get; set; }
}

public sealed class Item
{
    public required string Type { get; set; }
    public string? Id { get; init; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public Offers? Offers { get; set; }
    public ImageObject[]? Images { get; set; }
}

public sealed class ItemListElement
{
    public required string Type { get; set; }
    public string? Name { get; set; }
    public int? Position { get; set; }
    public Item? Item { get; set; }
}

/// <summary>
/// https://schema.org/isPartOf
/// </summary>
public sealed class IsPartOf
{
    public required string Type { get; set; }
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Url { get; set; }
}

public sealed class HasMerchantReturnPolicy
{
    public required string Type { get; set; }
    public string? ReturnPolicyCategory { get; set; }
    public int? MerchantReturnDays { get; set; }
    public string? ReturnFees { get; set; }
    public string? ReturnMethod { get; set; }
    public string? ApplicableCountry { get; set; }
}

/// <summary>
/// https://schema.org/logo
/// </summary>
// public sealed class Logo
// {
//     public required string Type { get; set; }
//     public required string Url { get; set; }
// }

/// <summary>
/// https://schema.org/offers
/// </summary>
public sealed class Offers
{
    public required string Type { get; init; }
    public string? Id { get; set; }
    public required string PriceCurrency { get; init; }
    public required decimal Price { get; init; }
    public required string Availability { get; init; }
    public required string ItemCondition { get; init; }
    public string? Url { get; set; }
    public required DateTime? PriceValidUntil { get; set; }
    public HasMerchantReturnPolicy? HasMerchantReturnPolicy { get; set; }
    public SchemaOrg? Seller { get; init; }
}

public sealed class PlaceModel
{
    public string? Name { get; init; }
    public PostalAddress? Address { get; init; }
}

/// <summary>
/// https://schema.org/potentialAction - Thing > Property :: potentialAction
/// https://validator.schema.org/
/// </summary>
public sealed class PotentialAction
{
    public required string Type { get; init; }
    public required string Target { get; init; }
    public string? Name { get; init; }
    public string? QueryInput { get; init; }
}

/// <summary>
/// https://schema.org/publisher
/// </summary>
public sealed class Publisher
{
    public required string Type { get; init; }
    public string? Id { get; set; }
    public string? Name { get; init; }
    public ImageObject? Logo { get; init; }
}

public sealed class Seller
{
    public required string Type { get; init; }
    public string? Id { get; set; }
    public string Name { get; init; }
    public string? Url { get; init; }
    public ImageObject? Logo { get; init; }
    public ContactPoint[]? ContactPoint { get; init; }
    public string[]? SameAs { get; set; } // https://schema.org/sameAs
}