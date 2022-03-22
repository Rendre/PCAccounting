using System.Text.Json;
using SharedKernel.Services;
namespace WebClient.Utils;

public class Util
{
    public static bool CheckToken(JsonElement? json, IRequestCookieCollection cookieCollection)
    {
    ILoginService loginService = new LoginService();
    string? token = null;
        if (json != null && json.Value.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();
        }
        else
        {
            if (cookieCollection.ContainsKey("token"))
            {
                cookieCollection.TryGetValue("token", out token);
            }
        }
        var isValid = loginService.IsSessionValid(token);
        return isValid;
    }
}