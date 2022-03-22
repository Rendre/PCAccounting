namespace SharedKernel.Utils;

public class Util
{
    public static bool CheckFileExtension(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLower();
        return ext.Equals(".jpeg") ||
               ext.Equals(".jpg") ||
               ext.Equals(".png") ||
               ext.Equals(".bmp");
    }
}