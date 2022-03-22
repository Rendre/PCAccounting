using DB.Entities;
using DB.Repositories.File;

namespace SharedKernel.Services;

public class DekstopSave : IFileSave
{
    private readonly IFileRepository _fileRepository = new FileDapperRepository();

    public void SaveItem(uint computerId, byte[] fileBytes, string filePath, string pathForSaveFile, string fileID, out Files? file)
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
        file = new Files() { ComputerId = computerId, Name = fileName, Path = pathForSaveFile };
        _fileRepository.SaveItem(file);
    }
}