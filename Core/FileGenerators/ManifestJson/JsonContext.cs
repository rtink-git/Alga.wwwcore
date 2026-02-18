using System.Text.Json.Serialization;

namespace Alga.wwwcore.Core.FileGenerators.ManifestJson;

[JsonSerializable(typeof(ManifestModel))]
[JsonSerializable(typeof(Icon))]
[JsonSerializable(typeof(Screenshot))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
internal partial class JsonContext : JsonSerializerContext { }
