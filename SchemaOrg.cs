using System.Text;

namespace Alga.wwwcore;

public class SchemaOrg
{
    Models.SchemaOrg _schemaOrg;
    public SchemaOrg(Models.SchemaOrg schemaOrg) => _schemaOrg = schemaOrg;

    public StringBuilder? GetJsonLD(string url, string? title = null, string? description = null)
    {
        if (_schemaOrg == null) return null;

        if (!string.IsNullOrEmpty(url)) _schemaOrg.Url = url;
        if (!string.IsNullOrEmpty(title)) _schemaOrg.Name = title;
        if (!string.IsNullOrEmpty(description)) _schemaOrg.Description = description;

        var jsonSb = new StringBuilder();

        jsonSb.Append("{");
        jsonSb.Append("\"@context\":\"https://schema.org\"");
        jsonSb.Append($",\"@type\":\"{_schemaOrg.Type}\"");
        if (_schemaOrg.Url != null) jsonSb.Append($",\"url\":\"{_schemaOrg.Url}\"");
        if (_schemaOrg.Name != null) jsonSb.Append($",\"name\":\"{_schemaOrg.Name}\"");
        if (_schemaOrg.DatePublished.HasValue) jsonSb.Append($",\"datePublished\":\"{_schemaOrg.DatePublished?.ToString("yyyy-MM-ddTHH:mm:ss") }\"");
        if (_schemaOrg.DateModified.HasValue) jsonSb.Append($",\"dateModified\":\"{_schemaOrg.DateModified?.ToString("yyyy-MM-ddTHH:mm:ss") }\"");
        if (string.IsNullOrEmpty(_schemaOrg.Description)) jsonSb.Append($",\"description\":\"{_schemaOrg.Description}\"");
        if (string.IsNullOrEmpty(_schemaOrg.Email)) jsonSb.Append($",\"email\":\"{_schemaOrg.Email}\"");
        if (string.IsNullOrEmpty(_schemaOrg.LegalName)) jsonSb.Append($",\"legalName\":\"{_schemaOrg.LegalName}\"");
        if (string.IsNullOrEmpty(_schemaOrg.Logo)) jsonSb.Append($",\"logo\":\"{_schemaOrg.Logo}\"");
        if (string.IsNullOrEmpty(_schemaOrg.OperatingSystem)) jsonSb.Append($",\"operatingSystem\":\"{_schemaOrg.OperatingSystem}\"");
        if (string.IsNullOrEmpty(_schemaOrg.Telephone)) jsonSb.Append($",\"telephone\":\"{_schemaOrg.Telephone}\"");
        if (_schemaOrg.StartDate.HasValue) jsonSb.Append($",\"startDate\":\"{_schemaOrg.StartDate?.ToString("yyyy-MM-ddTHH:mm:ss") }\"");
        if (_schemaOrg.EndDate.HasValue) jsonSb.Append($",\"endDate\":\"{_schemaOrg.EndDate?.ToString("yyyy-MM-ddTHH:mm:ss") }\"");

        if (_schemaOrg.ApplicationCategory?.Length > 0)
        {
            var lAsStr = ""; foreach (var i in _schemaOrg.ApplicationCategory) lAsStr += $"\"{i}\","; lAsStr = lAsStr.Trim(',');
            jsonSb.Append($",\"applicationCategory\":[{lAsStr}]");
        }

        if (_schemaOrg.OpeningHours?.Length > 0)
        {
            var lAsStr = ""; foreach (var i in _schemaOrg.OpeningHours) lAsStr += $"\"{i}\","; lAsStr = lAsStr.Trim(',');
            jsonSb.Append($",\"openingHours\":[{lAsStr}]");
        }

        if (_schemaOrg.SameAs?.Length > 0)
        {
            var lAsStr = ""; foreach (var i in _schemaOrg.SameAs) lAsStr += $"\"{i}\","; lAsStr = lAsStr.Trim(',');
            jsonSb.Append($",\"sameAs\":[{lAsStr}]");
        }

        if (_schemaOrg.Screenshot?.Length > 0)
        {
            var lAsStr = ""; foreach (var i in _schemaOrg.Screenshot) lAsStr += $"\"{i}\","; lAsStr = lAsStr.Trim(',');
            jsonSb.Append($",\"screenshot\":[{lAsStr}]");
        }

        if (_schemaOrg.AddressModel != null)
        {
            jsonSb.Append(",\"address\":");
            jsonSb.Append("{");
            jsonSb.Append("\"@type\":\"PostalAddress\"");
            if (_schemaOrg.AddressModel.PostalCode != null) jsonSb.Append($",\"postalCode\":\"{_schemaOrg.AddressModel.PostalCode}\"");
            if (_schemaOrg.AddressModel.AddressCountry != null) jsonSb.Append($",\"addressCountry\":\"{_schemaOrg.AddressModel.AddressCountry}\"");
            if (_schemaOrg.AddressModel.AddressRegion != null) jsonSb.Append($",\"addressRegion\":\"{_schemaOrg.AddressModel.AddressRegion}\"");
            if (_schemaOrg.AddressModel.AddressLocality != null) jsonSb.Append($",\"addressLocality\":\"{_schemaOrg.AddressModel.AddressLocality}\"");
            if (_schemaOrg.AddressModel.StreetAddress != null) jsonSb.Append($",\"streetAddress\":\"{_schemaOrg.AddressModel.StreetAddress}\"");
            jsonSb.Append("}");
        }

        if (_schemaOrg.Author != null)
        {
            jsonSb.Append(",\"author\":");
            jsonSb.Append("{");
            jsonSb.Append($"\"@type\":\"{_schemaOrg.Author.Type}\"");
            jsonSb.Append($",\"name\":\"{_schemaOrg.Author.Name}\"");
            jsonSb.Append($",\"url\":\"{_schemaOrg.Author.Url}\"");
            jsonSb.Append("}");
        }

        if (_schemaOrg.GeoCordinates != null)
        {
            jsonSb.Append(",\"geo\":");
            jsonSb.Append("{");
            jsonSb.Append($"\"@type\":\"GeoCoordinates\"");
            jsonSb.Append($",\"latitude\":\"{_schemaOrg.GeoCordinates.latitude.ToString().Replace(",", ".")}\"");
            jsonSb.Append($",\"longitude\":\"{_schemaOrg.GeoCordinates.longitude.ToString().Replace(",", ".")}\"");
            jsonSb.Append("}");
        }

        if (_schemaOrg.PotentialAction != null)
        {
            jsonSb.Append(",\"potentialAction\":");
            jsonSb.Append("{");
            jsonSb.Append($"\"@type\":\"{_schemaOrg.PotentialAction.Type}\"");
            jsonSb.Append($",\"target\":\"{_schemaOrg.PotentialAction.Target}\"");
            if(!string.IsNullOrEmpty(_schemaOrg.PotentialAction.QueryInput)) jsonSb.Append($",\"query-input\":\"{_schemaOrg.PotentialAction.QueryInput}\"");
            jsonSb.Append("}");
        }

        if (_schemaOrg.Images?.Count > 0)
        {
            jsonSb.Append(",\"image\":");
            jsonSb.Append("[");

            foreach (var i in _schemaOrg.Images)
            {
                jsonSb.Append("{");
                jsonSb.Append($"\"@type\":\"{i.Type}\"");
                jsonSb.Append($"\"url\":\"{i.Url}\"");
                if(i.Width != null) jsonSb.Append($"\"width\":\"{i.Width}\"");
                if(i.Height != null) jsonSb.Append($"\"height\":\"{i.Height}\"");
                if(string.IsNullOrEmpty(i.Caption)) jsonSb.Append($"\"caption\":\"{i.Caption}\"");
                jsonSb.Append("}");
            }

            jsonSb.Append("]");
        }

        jsonSb.Append("}");

        return jsonSb;
    }
}
        //     if (imgFull is not null && model.ImageWidth > 0 && model.ImageHeight > 0)
        //     {
        //         jsonSb.Append(",\"image\":{\"@type\":\"ImageObject\",\"url\":\"")
        //             .Append(imgFull)
        //             .Append("\",\"width\":\"")
        //             .Append(model.ImageWidth.Value)
        //             .Append("\",\"height\":\"")
        //             .Append(model.ImageHeight.Value)
        //             .Append("\",\"caption\":\"")
        //             .Append(model.Title)
        //             .Append(" Image\"}");
        //     }