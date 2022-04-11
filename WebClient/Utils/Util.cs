using System.Text.Json;
using System.Text.Json.Serialization;
using SharedKernel.Services;
using WebClient.Models;

namespace WebClient.Utils;

public class Util
{
    public static bool CheckToken(JsonElement? json, IRequestCookieCollection? cookieCollection)
    {
        // это должно инжектится черз диай
        ILoginService loginService = new LoginService();
        string? token = null;
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
        var isValid = loginService.IsSessionValid(token);
        return isValid;
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