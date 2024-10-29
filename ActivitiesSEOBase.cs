namespace Alga.wwwcore;

public class ActivitiesSEOBase
{
    string _urlBase;
    string _ogSiteName;
    string? _twitterSite;  // example: @ElonMusk
    
    public ActivitiesSEOBase(string urlBase, string ogSiteName, string? twitterSite) {
        this._urlBase = urlBase;
        this._ogSiteName = ogSiteName;
        this._twitterSite = twitterSite; 
    }
    
    // -- Base SEO method

    public string SEO(string title, string? description = null, string? robot = null, string? urlCanonical = null, string? imageUrl = null)
    {
        var html = "";

        //-- base settings

        if (robot != null) html += "<meta name=\"robots\" content=\"" + robot + "\">";
        if (description != null) html += "<meta name=\"description\" content=\"" + description + "\">";
        if (urlCanonical != null) html += "<meta rel=\"canonical\" href=\"" + this._urlBase + urlCanonical + "\">";

        // -- og

        if (!string.IsNullOrEmpty(_ogSiteName) && !string.IsNullOrEmpty(title))
        {
            html += "<meta property=\"og:site_name\" content=\"" + _ogSiteName + "\">";
            html += "<meta property=\"og:type\" content=\"article\">";
            html += "<meta property=\"og:url\" content=\"" + this._urlBase + "\">";
            html += "<meta property=\"og:title\" content=\"" + title + "\">";
            if (!string.IsNullOrEmpty(description)) { html += "<meta property=\"og:description\" content=\"" + description + "\">"; }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                html += "<meta property=\"og:image\" content=\"" + this._urlBase + imageUrl + "\">";
                html += "<meta property=\"og:image:alt\" content=\"" + title + "\">";
            }
        }

        // -- twitter

        if (!string.IsNullOrEmpty(_twitterSite) && !string.IsNullOrEmpty(title))
        {
            html += "<meta name=\"twitter:site\" content=\"" + _twitterSite + "\">";
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