namespace DekstopClient.Utils;

internal class Util
{
    private static readonly HttpClient Client = new();

    public static async Task<string?> RequestHelper(byte[] buffer, string connectionAddress, string fileName, string pcID, string? fileID)
    {
        using var multipartFormContent = new MultipartFormDataContent();
        var strFile = Convert.ToBase64String(buffer);
        multipartFormContent.Add(new StringContent(strFile), name: "fileByString");
        multipartFormContent.Add(new StringContent(pcID), name: "computerID");
        multipartFormContent.Add(new StringContent(fileName), name: "fileName");
        if (!string.IsNullOrEmpty(fileID))
        {
            multipartFormContent.Add(new StringContent(fileID), "fileID");
        }

        var response = await Client.PostAsync(connectionAddress, multipartFormContent).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}