using System.Collections.Frozen;
using System.Text;

namespace Alga.wwwcore.Core.HtmlGenerator.PageGenerator;

sealed public class Req
{
    public required bool IsDebug { get; init; }
    public required StringBuilder BaseMetaSb { get; init; }
    public required SchemesGenerator.PageModel PageScheme { get; init; }
    public required FrozenDictionary<string, HashSet<string>> PagesModules { get; init; }
    public required SeoMetaGenerator.Req SeoMetaReq { get; init; }
    public string? PageModelAsJson { get; init; }
}