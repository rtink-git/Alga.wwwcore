using System.Collections.Frozen;
using System.Text;

namespace Alga.wwwcore.Core;

public interface IInitializer
{
    bool IsDebug { get; }
    FrozenDictionary<string, SchemesGenerator.PageModel> Pages { get; }
    FrozenDictionary<string, HashSet<string>> PagesModules { get; }
    StringBuilder BaseMetaHeadHtml { get; }
    string BaseUrl { get; }
    string AppNameShort { get; }
    string? AppTwitterSite { get; }
}