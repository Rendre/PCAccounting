namespace SharedKernel.Services.DownloadService;

public interface IFileDownload
{
    public void GetItem(uint id, out byte[]? pictureBytes, out string? fileType, out string? fileName);
}