namespace Alga.wwwcore;

public class ActivitiesSEOBase
{
    // -- Base SEO method

    public string SEO(string title, string? description = null, string? robot = null, string? urlCanonical = null, string? imageUrl = null)
    {
        var html = "";

        //-- base settings

        if (robot != null) html += "<meta name=\"robots\" content=\"" + robot + "\">";
        if (description != null) html += "<meta name=\"description\" content=\"" + description + "\">";
        if (urlCanonical != null) html += "<meta rel=\"canonical\" href=\"" + Config.url + urlCanonical + "\">";

        // -- og

        if (!string.IsNullOrEmpty(Config.name) && !string.IsNullOrEmpty(title))
        {
            html += "<meta property=\"og:site_name\" content=\"" + Config.name + "\">";
            html += "<meta property=\"og:type\" content=\"article\">";
            html += "<meta property=\"og:url\" content=\"" + Config.url + "\">";
            html += "<meta property=\"og:title\" content=\"" + title + "\">";
            if (!string.IsNullOrEmpty(description)) { html += "<meta property=\"og:description\" content=\"" + description + "\">"; }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                html += "<meta property=\"og:image\" content=\"" + Config.url + imageUrl + "\">";
                html += "<meta property=\"og:image:alt\" content=\"" + title + "\">";
            }
        }

        // -- twitter

        if (!string.IsNullOrEmpty(Config.TwitterSite) && !string.IsNullOrEmpty(title))
        {
            html += "<meta name=\"twitter:site\" content=\"" + Config.TwitterSite + "\">";
            html += "<meta name=\"twitter:card\" content=\"summary_large_image\">";
            html += "<meta name=\"twitter:description\" content=\"summary_large_image\">";
            html += "<meta name=\"twitter:title\" content=\"" + title + "\">";
            if (!string.IsNullOrEmpty(description)) { html += "<meta name=\"twitter:description\" content=\"" + description + "\">"; }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                html += "<meta property=\"name:twitter:image\" content=\"" + imageUrl + "\">";
                html += "<meta property=\"name:twitter:image:alt\" content=\"" + title + "\">";
            }
        }

        html += "<title>" + title + "</title>";

        return html;
    }
}

// Lines now: 63 - Before