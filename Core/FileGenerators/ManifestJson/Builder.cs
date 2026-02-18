using System.Text.Json;
using Alga.wwwcore.Core.FileGenerators.ManifestJson;

namespace Alga.wwwcore.Core.FileGenerators.ManifestJson;

internal sealed class Builder
{
    public void Do(Req req)
    {
        var filesToDelete = Directory.GetFiles(req.DirectoryPath, "*.json", SearchOption.TopDirectoryOnly)
            .Where(f => Path.GetFileName(f).Contains("manifest"));

        foreach (var file in filesToDelete)
            File.Delete(file);

        string filePath = Path.Combine(req.DirectoryPath, $"manifest.{req.Version}.json");

        var manifest = new ManifestModel
        {
            Name = req.Name,
            ShortName = req.NameShort,
            Description = req.Description,
            BackgroundColor = req.BackgroundColor,
            ThemeColor = req.ThemeColor,
            Icons = new[]
            {
                new Icon { Src = "/Modules/Total/content/Icon-192.png", Type = "image/png", Sizes = "192x192" },
                new Icon { Src = "/Modules/Total/content/Icon-512.png", Type = "image/png", Sizes = "512x512" }
            },
            Screenshots = new[]
            {
                new Screenshot
                {
                    Src = "/Modules/Total/content/screenshot-vertical.png",
                    Type = "image/png",
                    Sizes = "1080x1920",
                    Platform = "any",
                    Orientation = "portrait"
                },
                new Screenshot
                {
                    Src = "/Modules/Total/content/screenshot-horizontal.png",
                    Type = "image/png",
                    Sizes = "1920x1080",
                    Platform = "any",
                    Orientation = "landscape"
                }
            }
        };

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

        JsonSerializer.Serialize(fs, manifest, JsonContext.Default.ManifestModel);
    }
}