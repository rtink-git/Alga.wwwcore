using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Web;

namespace wwwcore.Helpers;
public static class TelegramInitDataValidator
{
    public class UserData
    {
        public long id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
        public bool allows_write_to_pm { get; set; }
        public string photo_url { get; set; }
    }

    public class InitData
    {
        public UserData user { get; set; }
        public string chat_instance { get; set; }
        public string chat_type { get; set; }
        public long auth_date { get; set; }
        public string hash { get; set; }
        public string signature { get; set; }
    }

    public static InitData Parse(string initData) {
        if (string.IsNullOrEmpty(initData)) throw new ArgumentNullException(nameof(initData));

        var parsed = HttpUtility.ParseQueryString(initData);
        
        if (parsed["user"] == null) throw new ArgumentException("Missing 'user' in initData");
        if (parsed["auth_date"] == null) throw new ArgumentException("Missing 'auth_date' in initData");

        return new InitData {
            user = JsonSerializer.Deserialize<UserData>(parsed["user"]!)!,
            chat_instance = parsed["chat_instance"] ?? string.Empty,
            chat_type = parsed["chat_type"] ?? string.Empty,
            auth_date = long.Parse(parsed["auth_date"]!),
            hash = parsed["hash"] ?? string.Empty,
            signature = parsed["signature"] ?? string.Empty
        };
}

    public static bool IsValid(string initData, string botToken)
    {
        if (string.IsNullOrEmpty(initData) || string.IsNullOrEmpty(botToken))
            return false;

        NameValueCollection parsed = HttpUtility.ParseQueryString(initData);
        string receivedHash = parsed["hash"];
        if (string.IsNullOrEmpty(receivedHash))
            return false;

        // Удаляем 'hash' из параметров
        parsed.Remove("hash");

        // Формируем data_check_string
        var dataCheckList = parsed.AllKeys
            .OrderBy(k => k)
            .Select(k => $"{k}={parsed[k]}")
            .ToArray();
        string dataCheckString = string.Join("\n", dataCheckList);

        // Вычисляем secret_key = HMAC_SHA256(botToken, "WebAppData")
        byte[] secretKey;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData")))
        {
            secretKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(botToken));
        }

        // Вычисляем HMAC_SHA256(data_check_string, secret_key)
        byte[] computedHash;
        using (var hmac = new HMACSHA256(secretKey))
        {
            computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
        }

        string hexHash = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();

        return hexHash == receivedHash;
    }
}