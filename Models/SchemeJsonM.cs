namespace Alga.wwwcore.Models;
internal sealed class SchemeJsonM
{
    public string? title { get; set; }
    public string? description { get; set; }
    public string? schemaType { get; set; } // https://schema.org type
    public string? path { get; set; }
    public string? robots { get; set; }
    public string? script { get; set; }
    public string? style { get; set; }
    public List<string>? modules { get; set; } // if dir - мы ищем внутри script.js & style.css, также вы можете определить просто относительный путь к файлам коммпонентов внутри wwwroot, например: /Modules/HeaderBox/script.js 
    public byte[]? html { get; set; }
};
