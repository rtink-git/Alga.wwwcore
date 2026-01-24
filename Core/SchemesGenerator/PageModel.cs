namespace Alga.wwwcore.Core.SchemesGenerator;

public sealed class PageModel
{
    public string? script { get; set; }
    public string? style { get; set; }
    public List<string>? modules { get; set; } // if dir - мы ищем внутри script.js & style.css, также вы можете определить просто относительный путь к файлам коммпонентов внутри wwwroot, например: /Modules/HeaderBox/script.js 
    public byte[]? Html { get; set; }
    public int HtmlCheckSum { get; set; } = 0;
}