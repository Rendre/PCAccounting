using System.Text.Json;
using System.Text.Json.Serialization;
using WebClient.Models;

namespace WebClient.Utils;

public class Util
{
    public static string GetToken(JsonElement? json, IRequestCookieCollection? cookieCollection)
    {
        string ? token = null;
        if (json != null && json.Value.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();
        }
        else
        {
            if (cookieCollection != null && cookieCollection.ContainsKey("token"))
            {
                cookieCollection.TryGetValue("token", out token);
            }
        }
        return token;
    }

    public static string SerializeToJson<T>(ResponceObject<T> responceObject)
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = new LowerCaseNamingPolicy()
        };

        var result = JsonSerializer.Serialize(responceObject, options);
        return result;
    }

    private class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            name.ToLower();
    }
}