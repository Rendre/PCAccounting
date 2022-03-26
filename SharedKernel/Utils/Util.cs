using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DB.Entities;

namespace SharedKernel.Utils;

public class Util
{
    private static ProjectProperties? ProjectProperties;

    public static ProjectProperties? GetProjectProperties()
    {
        if (ProjectProperties != null) return ProjectProperties;

        var directory = new DirectoryInfo(Environment.CurrentDirectory);
        var path = directory + "\\Properties\\Properties.json";
        var kek = File.ReadAllText(path);
        ProjectProperties = JsonSerializer.Deserialize<ProjectProperties>(kek);

        return ProjectProperties;
    }

    public static bool CheckFileExtension(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLower();
        return ext.Equals(".jpeg") ||
               ext.Equals(".jpg") ||
               ext.Equals(".png") ||
               ext.Equals(".bmp");
    }

    public static bool CheckEmail(string? email)
    {
        if (email == null) return false;

        return email.Contains('@') &&
               email.Contains('.');
    }

    public static string? Encode(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        using var algorithm = TripleDES.Create();
        var bytes1 = Encoding.UTF8.GetBytes(CryptKeys[0]);
        var bytes2 = Encoding.UTF8.GetBytes(CryptKeys[1].Substring(0, 8));

        using var transform = algorithm.CreateEncryptor(bytes1, bytes2);
        algorithm.Mode = CipherMode.ECB;
        var inputbuffer = Encoding.UTF8.GetBytes(text);
        var outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        return Convert.ToBase64String(outputBuffer);
    }

    private static readonly string[] CryptKeys =
    {
        "leGOzfYCO2qBPoZiAsHQvib4",
        "afZLuxKlPdV1EsdnQxFMALtZ",
    };

    public static string? CheckTelNumber(string? strNumber)
    {
        if (strNumber == null) return "";

        if (strNumber.Length is < 10 or > 12) return "";

        if (strNumber.StartsWith("9") && strNumber.Length == 10)
        {
            strNumber = "+7" + strNumber;
        }

        if (strNumber.StartsWith("8") && strNumber.Length == 11)
        {
            strNumber = "+7" + strNumber.Substring(1, 10);
        }

        if (strNumber.StartsWith("+7") && strNumber.Length == 12)
        {
            for (var i = 2; i < strNumber.Length; i++)
            {
                if (!char.IsDigit(strNumber[i])) return "";
            }
            return strNumber;

        }
        return "";
    }
}