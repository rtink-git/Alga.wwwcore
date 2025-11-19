using System.Text.Json.Nodes;

namespace Alga.wwwcore.Helpers;

/// <summary>
/// https://schema.org/docs/full.html?utm_source=chatgpt.com
/// https://developers.google.com/search/docs/appearance/structured-data/search-gallery?hl=ru
/// Google Structured Data Testing Tool: https://search.google.com/test/rich-results
/// https://validator.schema.org/
/// </summary>
public class SchemaOrgJsonBuilder : JsonBuilder
{
    public SchemaOrgJsonBuilder AddNested(string key, Action<SchemaOrgJsonBuilder> buildAction)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        var builder = new SchemaOrgJsonBuilder();
        buildAction(builder);
        Add(key, builder.BuildJsonObject());
        return this;
    }

    public SchemaOrgJsonBuilder WithContext() { Add("@context", "https://schema.org"); return this; }

    public SchemaOrgJsonBuilder WithBaseGroup(string type, string url, string idsub, string name)
    {
        WithType(type);
        WithId($"{url}{idsub}");
        WithUrl(url);
        WithName(name);

        return this;
    }

    public SchemaOrgJsonBuilder WithType(string? val) { AddString("@type", val); return this; }

    public SchemaOrgJsonBuilder WithTypeCollectionPage(
        string url,
        string name,
        JsonObject?[]? mainEntityArray = null,
        string? description = null,
        JsonObject? isPartOf = null,
        JsonObject? primaryImageOfPageAsImageObject = null
    )
    {
        WithBaseGroup("CollectionPage", url, "#collection", name);
        WithDescription(description);
        Add("mainEntity", mainEntityArray);
        Add("isPartOf", isPartOf);
        Add("primaryImageOfPage", primaryImageOfPageAsImageObject);
        return this;
    }

    public SchemaOrgJsonBuilder WithTypeOrganization(
        string url,
        string name,
        JsonNode? logoAsImageObject = null,
        string? description = null,
        string? legalName = null,
        string? alternateName = null,
        DateTime? foundingDate = null,
        string[]? sameAs = null,
        string? tel = null,
        string? email = null,
        string? areaServed = null
    )
    {
        WithBaseGroup("Organization", url, "#organization", name);

        WithDescription(description);
        Add("logo", logoAsImageObject);
        WithInLegalName(legalName);
        WithAlternateName(alternateName);
        WithFoundingDate(foundingDate);
        Add("sameAs", sameAs);
        AddNested("contactPoint", cp =>
        {
            cp.WithType("ContactPoint");
            cp.Add("contactType", "sales");
            cp.WithTelephone(tel);
            cp.WithEmail(email);
            cp.Add("areaServed", areaServed);
            cp.Add("availableLanguage", "Russian");
        });

        return this;
    }

    public SchemaOrgJsonBuilder WithTypeWebSite(
        string url,
        string name,
        string? description = null,
        string? inLanguage = null,
        JsonNode? logoAsImageObject = null,
        string? isPartOf = null,
        int? copyrightYear = null,
        string? copyrightHolder = null,
        string? urlSearch = null,
        string? inpSearch = null,
        string? placceholderSearch = null
    )
    {
        WithBaseGroup("WebSite", url, "#website", name);

        WithDescription(description);
        WithInLanguage(inLanguage);
        Add("logo", logoAsImageObject);
        if (!string.IsNullOrWhiteSpace(isPartOf)) AddNested("isPartOf", isp => { isp.WithId(isPartOf); });
        WithCopyrightYear(copyrightYear);
        WithCopyrightHolder(copyrightHolder);
        if (urlSearch != null && inpSearch != null) AddArray("potentialAction", new SchemaOrgJsonBuilder().WithSearchAction($"{urlSearch}", placceholderSearch, true, inpSearch).BuildJsonObject());

        return this;
    }

    public SchemaOrgJsonBuilder WithId(string? val) { AddString("@id", val); return this; }

    // -----

    public SchemaOrgJsonBuilder WithAlternateName(string? val) { AddString("alternateName", val); return this; }

    public SchemaOrgJsonBuilder WithApplicableCountry(string? val) { AddString("applicableCountry", val); return this; }

    public SchemaOrgJsonBuilder WithAvailability(string? val) { AddString("availability", val); return this; }

    public SchemaOrgJsonBuilder WithBestRating(decimal? val) { AddDecimal("bestRating", val); return this; }

    public SchemaOrgJsonBuilder WithCaption(string? val) { AddString("caption", val); return this; }

    public SchemaOrgJsonBuilder WithCopyrightHolder(string? val) { AddString("copyrightHolder", val); return this; }

    public SchemaOrgJsonBuilder WithCopyrightYear(int? val) { AddInt("copyrightYear", val); return this; }

    public SchemaOrgJsonBuilder WithCurrenciesAccepted(string? val) { AddString("currenciesAccepted", val); return this; }

    public SchemaOrgJsonBuilder WithDescription(string? val) { AddString("description", val); return this; }

    public SchemaOrgJsonBuilder WithEmail(string? val) { AddString("email", val); return this; }

    public SchemaOrgJsonBuilder WithEncodingFormat(string? val) { AddString("encodingFormat", val); return this; }

    public SchemaOrgJsonBuilder WithFoundingDate(DateTime? val) { AddString("foundingDate", val?.ToString("yyyy-MM-dd")); return this; }

    public SchemaOrgJsonBuilder WithItemCondition(string? val) { AddString("itemCondition", val); return this; }

    public SchemaOrgJsonBuilder WithHasMap(string? val) { AddString("hasMap", val); return this; }

    public SchemaOrgJsonBuilder WithHeight(int? val) { AddInt("height", val); return this; }

    public SchemaOrgJsonBuilder WithInLanguage(string? val) { AddString("inLanguage", val); return this; }

    public SchemaOrgJsonBuilder WithInLegalName(string? val) { AddString("legalName", val); return this; }

    public SchemaOrgJsonBuilder WithMerchantReturnDays(int? val) { AddInt("merchantReturnDays", val); return this; }

    public SchemaOrgJsonBuilder WithName(string? val) { AddString("name", val); return this; }

    public SchemaOrgJsonBuilder WithPosition(int? val) { AddInt("position", val); return this; }

    public SchemaOrgJsonBuilder WithPrice(decimal? val) { AddDecimal("price", val); return this; }

    public SchemaOrgJsonBuilder WithPriceCurrency(string? val) { AddString("priceCurrency", val); return this; }

    public SchemaOrgJsonBuilder WithPriceRange(string? val) { AddString("priceRange", val); return this; }

    public SchemaOrgJsonBuilder WithReviewBody(string? val) { AddString("reviewBody", val); return this; }

    public SchemaOrgJsonBuilder WithReviewCount(int? val) { AddInt("reviewCount", val); return this; }

    public SchemaOrgJsonBuilder WithReviewRating(decimal? val) { AddDecimal("reviewRating", val); return this; }

    public SchemaOrgJsonBuilder WithRatingValue(decimal? val) { AddDecimal("ratingValue", val); return this; }

    public SchemaOrgJsonBuilder WithRepresentativeOfPage(bool? val) { AddBool("representativeOfPage", val); return this; }

    public SchemaOrgJsonBuilder WithReturnFees(string? val) { AddString("returnFees", val); return this; }

    public SchemaOrgJsonBuilder WithReturnMethod(string? val) { AddString("returnMethod", val); return this; }

    public SchemaOrgJsonBuilder WithReturnPolicyCategory(string? val) { AddString("returnPolicyCategory", val); return this; }

    public SchemaOrgJsonBuilder WithSku(string? val) { AddString("sku", val); return this; }

    public SchemaOrgJsonBuilder WithTelephone(string? val) { AddString("telephone", val?.Replace(" ", null).Replace("(", "-").Replace(")", "-")); return this; }

    public SchemaOrgJsonBuilder WithUrl(string? val) { AddString("url", val); return this; }

    public SchemaOrgJsonBuilder WithWidth(int? val) { AddInt("width", val); return this; }

    public SchemaOrgJsonBuilder WithWorstRating(decimal? val) { AddDecimal("worstRating", val); return this; }

    // -----

    public SchemaOrgJsonBuilder WithAggregateRating(
        Rating rating,
        int reviewCount)
    {
        WithType("AggregateRating");
        WithReviewCount(reviewCount);
        //AddNested("reviewRating", i => i.WithRating(rating));
        WithRatingValue(rating.ratingValue);
        WithBestRating(rating.bestRating);
        WithWorstRating(rating.worstRating);
        return this;
    }

    public SchemaOrgJsonBuilder WithImageObject(
        string url,
        int? width = null,
        int? height = null,
        string? caption = null,
        string? encodingFormat = null,
        bool? representativeOfPage = null)
    {
        WithType("ImageObject");
        WithUrl(url);
        WithWidth(width);
        WithHeight(height);
        WithCaption(caption);
        WithEncodingFormat(encodingFormat);
        WithRepresentativeOfPage(representativeOfPage);
        return this;
    }

    public SchemaOrgJsonBuilder WithPerson (
        string? name)
    {
        WithType("Person");
        WithName(name);
        return this;
    }

    public SchemaOrgJsonBuilder WithRating(Rating rating)
    {
        WithType("Rating");
        WithRatingValue(rating.ratingValue);
        WithBestRating(rating.bestRating);
        WithWorstRating(rating.worstRating);
        return this;
    }

    public SchemaOrgJsonBuilder WithReview (
        Rating rating,
        string? reviewBody,
        DateTime? datePublished,
        string? authorName)
    {
        WithType("Review");
        AddNested("reviewRating", i => i.WithRating(rating));
        WithReviewBody(reviewBody);
        if(datePublished != null)
            Add("datePublished", datePublished.Value.ToString("yyyy-MM-dd"));
        if(authorName != null)
            AddNested("author", i => i.WithPerson(authorName));
        return this;
    }

    public SchemaOrgJsonBuilder WithSearchAction(
        string urlTemplate,
        string? name = null,
        bool valueRequired = true,
        string valueName = "search_term_string")
    {
        WithType("SearchAction");

        if (!string.IsNullOrEmpty(name))
        {
            WithName(name);
        }

        var target = new JsonObject
        {
            ["@type"] = "EntryPoint",
            ["urlTemplate"] = urlTemplate
        };
        Add("target", target);

        // Query-input object  
        var queryInput = new JsonObject
        {
            ["@type"] = "PropertyValueSpecification",
            ["valueRequired"] = valueRequired,
            ["valueName"] = valueName
        };
        Add("query-input", queryInput);

        return this;
    }

    public SchemaOrgJsonBuilder WithOpeningHoursSpecification(
        string? dayOfWeek = null,
        string? opens = null,
        string? closes = null,
        bool? closed = null,
        string? validFrom = null,
        string? validThrough = null)
    {
        WithType("OpeningHoursSpecification");

        if (!string.IsNullOrWhiteSpace(dayOfWeek))
            Add("dayOfWeek", dayOfWeek);

        if (!string.IsNullOrWhiteSpace(opens))
            Add("opens", opens);

        if (!string.IsNullOrWhiteSpace(closes))
            Add("closes", closes);

        if (closed.HasValue)
            Add("closed", closed.Value);

        if (!string.IsNullOrWhiteSpace(validFrom))
            Add("validFrom", validFrom);

        if (!string.IsNullOrWhiteSpace(validThrough))
            Add("validThrough", validThrough);

        return this;
    }

    public SchemaOrgJsonBuilder WithPostalAddress(
        string? streetAddress = null,
        string? addressLocality = null,
        string? addressRegion = null,
        string? postalCode = null,
        string? addressCountry = null,
        string? poBox = null,
        string? name = null,
        string? addressType = null,
        string? sameAs = null)
    {
        WithType("PostalAddress");

        if (!string.IsNullOrWhiteSpace(streetAddress))
            Add("streetAddress", streetAddress);

        if (!string.IsNullOrWhiteSpace(addressLocality))
            Add("addressLocality", addressLocality);

        if (!string.IsNullOrWhiteSpace(addressRegion))
            Add("addressRegion", addressRegion);

        if (!string.IsNullOrWhiteSpace(postalCode))
            Add("postalCode", postalCode);

        if (!string.IsNullOrWhiteSpace(addressCountry))
            Add("addressCountry", addressCountry);

        if (!string.IsNullOrWhiteSpace(poBox))
            Add("poBox", poBox);

        if (!string.IsNullOrWhiteSpace(name))
            Add("name", name);

        if (!string.IsNullOrWhiteSpace(addressType))
            Add("addressType", addressType);

        if (!string.IsNullOrWhiteSpace(sameAs))
            Add("sameAs", sameAs);

        return this;
    }

    public SchemaOrgJsonBuilder WithGeoCoordinates(
        double latitude,
        double longitude,
        double? altitude = null,
        string? accuracy = null,
        string? placeName = null,
        string? region = null,
        SchemaOrgJsonBuilder? address = null)
    {
        WithType("GeoCoordinates");

        Add("latitude", latitude);
        Add("longitude", longitude);

        if (altitude.HasValue)
            Add("altitude", altitude.Value);

        if (!string.IsNullOrWhiteSpace(accuracy))
            Add("accuracy", accuracy);

        if (!string.IsNullOrWhiteSpace(placeName))
            Add("placeName", placeName);

        if (!string.IsNullOrWhiteSpace(region))
            Add("region", region);

        if (address != null)
            Add("address", address.BuildJsonObject());

        return this;
    }


    public class Rating
    {
        public required decimal ratingValue { get; init; }
        public required decimal bestRating { get; init; }
        public required decimal worstRating { get; init; }
    }

}
