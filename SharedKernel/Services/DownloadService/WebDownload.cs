using DB.Repositories;

namespace SharedKernel.Services.DownloadService;

public class WebDownload : IFileDownload
{
    private readonly IUnitOfWork _unitOfWork;
    public WebDownload(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void GetItem(uint id, out byte[]? pictureBytes, out string? fileType, out string? fileName)
    {
        var item = _unitOfWork.FileRepository.GetItem(id);
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