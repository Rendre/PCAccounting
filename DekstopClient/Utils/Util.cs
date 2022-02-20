using System.Security.Cryptography;
using System.Text;

namespace DekstopClient.Utils;

public class Util
{
    public static string Encode(string text)
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

    public static string CheckTelNumber(string strNumber)
    {
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
