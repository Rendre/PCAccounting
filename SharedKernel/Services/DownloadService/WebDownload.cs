using DB.Repositories.Files;

namespace SharedKernel.Services.DownloadService;

public class WebDownload : IFileDownload
{
    private readonly IFileRepository _fileRepository;

    public WebDownload(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public void GetItem(uint id, out byte[]? pictureBytes, out string? fileType, out string? fileName)
    {
        var item = _fileRepository.GetItem(id);
        if (item == null)
        {
            pictureBytes = null;
            fileType = null;
            fileName = null;
            return;
        }

        var path = Path.Combine(item.Path!);
        var extension = Path.GetExtension(item.Path);
        pictureBytes = File.ReadAllBytes(path);
        fileType = "image/" + extension;
        fileName = item.FileName;
    }

}