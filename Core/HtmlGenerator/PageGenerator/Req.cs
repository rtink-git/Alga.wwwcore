using System.Collections.Frozen;
using System.Text;

namespace Alga.wwwcore.Core.HtmlGenerator.PageGenerator;

sealed public class Req
{
    public required bool IsDebug { get; set; }
    public required StringBuilder BaseMetaSb { get; set; }
    public required SchemesGenerator.PageModel PageScheme { get; set; }
    public required FrozenDictionary<string, HashSet<string>> PagesModules { get; set; }
    public required SeoMetaGenerator.Req SeoMetaReq { get; set; }
    public string? PageModelAsJson { get; set; }
}