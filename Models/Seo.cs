namespace Alga.wwwcore.Models;

public sealed class Seo
{
    public string? Title { get; set; }
    public string? Path { get; set; }
    public string? UrlCanonical { get; set; }
    public string? Description { get; set; }
    public string? Robot { get; set; }
    public string? ImageUrl { get; set; }
    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
    public string? Lang { get; set; }
    public DateTime? DatePublished { get; set; }
    public string? SchemaType { get; set; } // https://schema.org type
    public string? Telephone { get; set; } // https://schema.org
    public string? Email { get; set; } // https://schema.org
    public string? AddressType { get; set; } // https://schema.org
    public int? PostalCode { get; set; } // https://schema.org
    public string? AddressCountry { get; set; } // https://schema.org
    public string? AddressRegion { get; set; } // https://schema.org
    public string? AddressLocality { get; set; } // https://schema.org
    public string? StreetAddress { get; set; } // https://schema.org
    public string? GeoType { get; set; } // https://schema.org
    public double? GeoLatitude { get; set; } // https://schema.org
    public double? GeoLongitude { get; set; } // https://schema.org
    public string[]? OpeningHours { get; set; } // https://schema.org
}
