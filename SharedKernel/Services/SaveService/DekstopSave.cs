using DB.Entities;
using DB.Repositories;

namespace SharedKernel.Services.SaveService;

public class DekstopSave : IFileSave
{
    private readonly IUnitOfWork _unitOfWork;
    public DekstopSave(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void SaveItem(uint computerID, byte[] fileBytes, string filePath, string pathForSaveFile, string fileID, out FileEntity? file)
    {
        var fileName = Path.GetFileName(filePath);
        var ext = Path.GetExtension(filePath);
        var guidFileName = Guid.NewGuid().ToString("N");
        if (!Directory.Exists(pathForSaveFile))
        {
            Directory.CreateDirectory(pathForSaveFile);
        }
        pathForSaveFile = pathForSaveFile + guidFileName + ext;

        using var fileStream = new FileStream(pathForSaveFile, FileMode.CreateNew);
        fileStream.Write(fileBytes);
        file = new FileEntity { ComputerID = computerID, FileName = fileName, Path = pathForSaveFile };
        _unitOfWork.FileRepository.SaveItem(file);
    }
}