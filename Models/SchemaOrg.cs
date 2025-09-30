namespace Alga.wwwcore.Models;

/// <summary>
/// https://schema.org/docs/full.html?utm_source=chatgpt.com
/// </summary>
public sealed class SchemaOrg
{
    public string? Type { get; set; } // https://schema.org type
    public string? Url { get; set; } // https://schema.org/url
    public string? Name { get; set; } // https://schema.org/name
    public string? Description { get; set; } // https://schema.org/description
    public string? Email { get; set; } // https://schema.org/email
    public string? LegalName { get; set; } // https://schema.org/legalName
    public string? Logo { get; set; } // https://schema.org/logo
    public string? OperatingSystem { get; set; } // https://schema.org/operatingSystem
    public string? Telephone { get; set; } // https://schema.org/telephone
    public DateTime? DatePublished { get; set; } // https://schema.org/datePublished
    public DateTime? DateModified { get; set; } // https://schema.org/dateModified
    public DateTime? StartDate { get; set; } // https://schema.org/startDate
    public DateTime? EndDate { get; set; } // https://schema.org/endDate
    public string[]? ApplicationCategory { get; set; } // https://schema.org/applicationCategory
    public string[]? OpeningHours { get; set; } // https://schema.org/openingHours
    public string[]? SameAs { get; set; } // https://schema.org/sameAs
    public string[]? Screenshot { get; set; } // https://schema.org/screenshot
    public AddressModel? AddressModel { get; set; }
    public Author? Author { get; set; }
    public GeoCordinates? GeoCordinates { get; set; }
    public PotentialAction? PotentialAction { get; set; }
    public List<Image>? Images { get; set; }
}

/// <summary>
/// https://schema.org/address - Thing > Property :: address :: PostalAddress
/// </summary>
public sealed class AddressModel
{
    public string? AddressCountry { get; set; }
    public string? AddressRegion { get; set; }
    public string? AddressLocality { get; set; }
    public string? StreetAddress { get; set; }
    public string? PostalCode { get; set; }
}

public sealed class Author
{
    public required string Type { get; set; }
    public required string Name { get; set; }
    public string? Url { get; set; }
}

/// <summary>
/// https://schema.org/image
/// </summary>
public sealed class Image
{
    public required string Type { get; set; } = "ImageObject";
    public required string Url { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? Caption { get; set; }
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
/// https://schema.org/potentialAction - Thing > Property :: potentialAction
/// </summary>
public sealed class PotentialAction
{
    public required string Type { get; init; }
    public required string Target { get; init; }
    public string? QueryInput { get; init; }
}