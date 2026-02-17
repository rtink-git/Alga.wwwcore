
using System.Text.Json.Serialization;

namespace Alga.wwwcore.Core.SchemesGenerator;

[JsonSerializable(typeof(PageModel))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
internal partial class PageModelJsonContext : JsonSerializerContext { }