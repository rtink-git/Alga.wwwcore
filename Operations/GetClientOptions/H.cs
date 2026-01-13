namespace Alga.wwwcore.Operations.GetClientOptions;

internal static class H
{
    public static Res Do(ClientOptions? req)
    {
        if (req is null) throw new InvalidOperationException("Request configuration is missing. Builder.Do cannot proceed with a null request.");
        if (string.IsNullOrEmpty(req.BaseUrl)) throw new InvalidOperationException($"{nameof(req.BaseUrl)} is missing or empty. Request configuration is invalid.");
        if (string.IsNullOrEmpty(req.Name)) throw new InvalidOperationException($"{nameof(req.Name)} is missing or empty. Request configuration is invalid.");
        if (string.IsNullOrEmpty(req.NameShort)) throw new InvalidOperationException($"{nameof(req.NameShort)} is missing or empty. Request configuration is invalid.");
        if (string.IsNullOrEmpty(req.Description)) throw new InvalidOperationException($"{nameof(req.Description)} is missing or empty. Request configuration is invalid.");
        if (string.IsNullOrEmpty(req.BackgroundColor)) throw new InvalidOperationException($"{nameof(req.BackgroundColor)} is missing or empty. Request configuration is invalid.");
        if (string.IsNullOrEmpty(req.ThemeColor)) throw new InvalidOperationException($"{nameof(req.ThemeColor)} is missing or empty. Request configuration is invalid.");

        return new Res(
            req.BaseUrl,
            req.Name,
            req.NameShort,
            req.Description,
            req.BackgroundColor,
            req.ThemeColor,
            req.PreconnectUrls,
            req.CacheUrls,
            req.UseTelegram,
            req.UseMessagePack,
            req.BingSiteVerification,
            req.TwitterSite,
            req.YandexVerificationCode,
            req.YandexMetrikaCode,
            req.GoogleAnalyticsCode,
            req.GoogleFontsUrl,
            req.GoogleSiteVerification,
            req.OfflinePageUrlPath);
    }
}