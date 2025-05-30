using System.Text.Json;

namespace Alga.wwwcore;

class _ManifestJson
{
    Root.ConfigModel _ConfigM { get; }
    internal _ManifestJson(Root.ConfigModel config) => this._ConfigM = config;

    internal void Build() {
        var manifestModel = new ManifestM {
            name = this._ConfigM.Name,
            short_name = this._ConfigM.NameShort,
            description = this._ConfigM.Description,
            background_color = this._ConfigM.BackgroundColor,
            theme_color = this._ConfigM.ThemeColor,
            icons = new List<IconM> {
                new IconM("/Modules/Total/content/Icon-192.png", "image/png", "192x192"),
                new IconM("/Modules/Total/content/Icon-512.png", "image/png", "512x512")
            },
            screenshots = new List<ScreenshotM> {
                new ScreenshotM("/Modules/Total/content/screenshot-vertical.png", "image/png", "1080x1920", "any", "portrait"),
                new ScreenshotM("/Modules/Total/content/screenshot-horizontal.png", "image/png", "1920x1080", "any", "landscape")
            }
        };

        var url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "manifest.json");
        if(File.Exists(url)) File.Delete(url);
        using(FileStream createStream = File.Create(url)) { JsonSerializer.Serialize(createStream, manifestModel); }
    }

    class ManifestM {
        public string id { get; set; } = "/";
        public string scope { get; set; } = "/";
        public string start_url { get; set; } = "/";
        public string display { get; set; } = "standalone";
        public string name { get; set; } = "";
        public string short_name { get; set; } = "";
        public string description { get; set; } = "";
        public string background_color { get; set; } = "#FFFFFF";
        public string theme_color { get; set; } = "#FFFFFF";
        public List<IconM>? icons { get; set; }
        public List<ScreenshotM>? screenshots { get; set; }
    }

    record IconM ( string src, string type, string sizes );
    record ScreenshotM ( string src, string type, string sizes, string platform, string orientation );
}