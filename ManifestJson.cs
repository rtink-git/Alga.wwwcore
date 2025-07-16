using System.Text.Json;
using System.Text.Json.Serialization;

namespace Alga.wwwcore;

public sealed class ManifestJson
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly Models.Config _config;

    public ManifestJson(Models.Config config) => _config = config;

    public void Build()
    {
        string filePath = Path.Combine("wwwroot", "manifest.json");
        
        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        
        JsonSerializer.Serialize(fs, new
        {
            id = "/",
            scope = "/",
            start_url = "/",
            display = "standalone",
            name = _config.Name,
            short_name = _config.NameShort,
            description = _config.Description,
            background_color = _config.BackgroundColor,
            theme_color = _config.ThemeColor,
            icons = new[]
            {
                new { src = "/Modules/Total/content/Icon-192.png", type = "image/png", sizes = "192x192" },
                new { src = "/Modules/Total/content/Icon-512.png", type = "image/png", sizes = "512x512" }
            },
            screenshots = new[]
            {
                new { 
                    src = "/Modules/Total/content/screenshot-vertical.png", 
                    type = "image/png", 
                    sizes = "1080x1920", 
                    platform = "any", 
                    orientation = "portrait" 
                },
                new { 
                    src = "/Modules/Total/content/screenshot-horizontal.png", 
                    type = "image/png", 
                    sizes = "1920x1080", 
                    platform = "any", 
                    orientation = "landscape" 
                }
            }
        }, _jsonOptions);
    }
}