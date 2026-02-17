using System.Text.Json.Serialization;

namespace Alga.wwwcore;

[JsonSerializable(typeof(SeoPageOptions))]
[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class SeoPageOptionsJsonContext : JsonSerializerContext { }