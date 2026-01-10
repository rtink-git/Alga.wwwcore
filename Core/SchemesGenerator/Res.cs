namespace Alga.wwwcore.Core.SchemesGenerator;

public readonly record struct Res(Dictionary<string, PageModel> Pages, Dictionary<string, HashSet<string>> PagesModules);

