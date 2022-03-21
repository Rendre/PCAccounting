namespace DekstopClient.Utils;

internal class Util
{
    private static readonly HttpClient Client = new();

    public static async Task<string?> RequestHelper(byte[] buffer, string connectionAddress, string fileName, string pcID, string? pictureID)
    {
        using var multipartFormContent = new MultipartFormDataContent();
        var strPicture = Convert.ToBase64String(buffer);
        multipartFormContent.Add(new StringContent(strPicture), name: "pictureByString");
        multipartFormContent.Add(new StringContent(pcID), name: "computerId");
        multipartFormContent.Add(new StringContent(fileName), name: "fileName");
        if (!string.IsNullOrEmpty(pictureID))
        {
            multipartFormContent.Add(new StringContent(pictureID), "pictureId");
        }

        var response = await Client.PostAsync(connectionAddress, multipartFormContent).ConfigureAwait(false);
        var kek = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return kek;
    }
}