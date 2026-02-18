namespace Alga.wwwcore.Core.FileGenerators.ManifestJson;

internal sealed class ManifestModel
{
    public string Id { get; set; } = "/";
    public string Scope { get; set; } = "/";
    public string StartUrl { get; set; } = "/";
    public string Display { get; set; } = "standalone";
    public string Name { get; set; } = "";
    public string ShortName { get; set; } = "";
    public string Description { get; set; } = "";
    public string BackgroundColor { get; set; } = "";
    public string ThemeColor { get; set; } = "";
    public Icon[] Icons { get; set; } = Array.Empty<Icon>();
    public Screenshot[] Screenshots { get; set; } = Array.Empty<Screenshot>();
}

internal sealed class Icon
{
    public string Src { get; set; } = "";
    public string Type { get; set; } = "";
    public string Sizes { get; set; } = "";
}

internal sealed class Screenshot
{
    public string Src { get; set; } = "";
    public string Type { get; set; } = "";
    public string Sizes { get; set; } = "";
    public string Platform { get; set; } = "";
    public string Orientation { get; set; } = "";
}