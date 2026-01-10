namespace Alga.wwwcore;

static class ClientOptionsValidator
{
    public static void Do(ClientOptions options)
    {
        const string subextext = $"{nameof(ClientOptions)} exception.";

        ArgumentNullException.ThrowIfNull(options, nameof(options));

        ValidateBaseUrl(options.BaseUrl, subextext);

        if (string.IsNullOrWhiteSpace(options.Name)) throw new ArgumentException($"{subextext} '{nameof(options.Name)}' of your app cannot be null or empty.", nameof(options.Name));
        if (string.IsNullOrWhiteSpace(options.NameShort)) throw new ArgumentException($"{subextext} '{nameof(options.NameShort)}' of your app cannot be null or empty.", nameof(options.NameShort));
        if (string.IsNullOrWhiteSpace(options.Description)) throw new ArgumentException($"{subextext} '{nameof(options.Description)}' of your app cannot be null or empty.", nameof(options.Description));
    }

    public static void ValidateBaseUrl(string url, string subextext)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException($"{subextext} Url cannot be null or empty.", nameof(url));
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) throw new ArgumentException($"{subextext} - '{url}' is not a valid absolute URI.", nameof(url));
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) throw new ArgumentException($"{subextext} - URI scheme must be 'http' or 'https', not '{uri.Scheme}'.", nameof(url));
        if (!string.IsNullOrEmpty(uri.Query)) throw new ArgumentException($"{subextext} - Url should not contain query parameters.", nameof(url));
        if (!string.IsNullOrEmpty(uri.Fragment)) throw new ArgumentException($"{subextext} - Url should not contain fragments (#).", nameof(url));
    }
}
