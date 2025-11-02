using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Alga.wwwcore.Helpers;

public class JsonBuilder
{
    protected readonly Dictionary<string, object?> _properties = new();

    public JsonBuilder Add(string key, object? value)
    {
        if (!string.IsNullOrEmpty(key) && value is not null)
            _properties[key] = value;
        return this;
    }
    public JsonBuilder AddBool(string key, bool? value)
    {
        if (value.HasValue)
            _properties[key] = value.Value;
        return this;
    }

    public JsonBuilder AddDecimal(string key, decimal? value)
    {
        if (value.HasValue)
            _properties[key] = value.Value;
        return this;
    }
    
    public JsonBuilder AddString(string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _properties[key] = value;
        return this;
    }

    public JsonBuilder AddInt(string key, int? value)
    {
        if (value.HasValue)
            _properties[key] = value;
        return this;
    }

    public JsonBuilder AddNested(string key, Action<JsonBuilder> buildAction)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        var builder = new JsonBuilder();
        buildAction(builder);
        _properties[key] = builder.BuildJsonObject();
        return this;
    }

    public JsonBuilder AddArray(string key, params object?[] items)
    {
        if (string.IsNullOrEmpty(key))
            return this;

        var array = new JsonArray(items.Select(i => ConvertToJsonNode(i)).ToArray()); // ✅ исправлено
        _properties[key] = array;
        return this;
    }

    public JsonBuilder AddArrayFrom<T>(string key, IEnumerable<T?> source)
    {
        if (string.IsNullOrEmpty(key))
            return this;

        var array = new JsonArray(source.Select(i => ConvertToJsonNode(i)).ToArray()); // ✅ исправлено
        _properties[key] = array;
        return this;
    }

    public JsonBuilder AddDictionary(IDictionary<string, object?> dict)
    {
        foreach (var (key, value) in dict)
            if (!string.IsNullOrEmpty(key))
                _properties[key] = value;
        return this;
    }

    public JsonBuilder AddDate(string key, DateTime date)
        => Add(key, date.ToString("yyyy-MM-dd"));

    public JsonBuilder AddDateTime(string key, DateTime dateTime)
        => Add(key, dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));

    public JsonObject BuildJsonObject()
    {
        var obj = new JsonObject();
        foreach (var (key, value) in _properties)
            obj[key] = ConvertToJsonNode(value);
        return obj;
    }

    public string ToJson(bool indented = true)
    {
        // var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        // {
        //     WriteIndented = indented,
        //     PropertyNamingPolicy = null,
        //     TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        // };

        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            PropertyNamingPolicy = null,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver() // Добавьте эту строку
        };

        // var options = new JsonSerializerOptions
        // {
        //     WriteIndented = indented,
        //     PropertyNamingPolicy = null,
        //     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //     TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        //     // Дополнительные настройки для агрессивной минификации
        //     Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        // };
        
        var json = BuildJsonObject().ToJsonString(options);
        
        // // Агрессивная минификация - удаляем ВСЕ пробелы вокруг значений
        // if (!indented)
        // {
        //     json = System.Text.RegularExpressions.Regex.Replace(json, @"\s+", " ");
        //     json = System.Text.RegularExpressions.Regex.Replace(json, @":\s+", ":");
        //     json = System.Text.RegularExpressions.Regex.Replace(json, @",\s+", ",");
        // }
        
        return json;
    }

    private static JsonNode? ConvertToJsonNode(object? value)
    {
        if (value is null)
            return null;

        return value switch
        {
            JsonNode node => node,
            JsonElement element => JsonNode.Parse(element.GetRawText()),
            IDictionary<string, object?> dict => new JsonBuilder().AddDictionary(dict).BuildJsonObject(),
            IEnumerable<object?> enumerable => new JsonArray(enumerable.Select(v => ConvertToJsonNode(v)).ToArray()),
            _ => JsonValue.Create(value)
        };
    }

    public override string ToString() => ToJson();
}
