using DB.Entities;
using DB.Repositories.Files;

namespace SharedKernel.Services;

public class WebSave : IFileSave
{
    private readonly IFileRepository _fileRepository = new FileDapperRepository();
    //todo:
    public void SaveItem(uint computerID, byte[] fileBytes, string fileName, string pathForSaveFile, string fileID,out FileEntity? file)
    {
        if (string.IsNullOrEmpty(fileID))
        {
            file = null;
            var guidFileName = Guid.NewGuid().ToString("N");
            var extension = Path.GetExtension(fileName);
            if (!Directory.Exists(pathForSaveFile))
            {
                Directory.CreateDirectory(pathForSaveFile);
            }

            pathForSaveFile = pathForSaveFile + guidFileName + extension;
        }
        else
        {
            var kekID = Convert.ToUInt32(fileID);
            file = _fileRepository.GetItem(kekID);
            if (file == null) return;

            pathForSaveFile = file.Path!;
        }

        using var fileStream = new FileStream(pathForSaveFile, FileMode.Append, FileAccess.Write);
        fileStream.Write(fileBytes);
        if (file == null)
        {
            file = new FileEntity { ComputerID = computerID, Name = fileName, Path = pathForSaveFile };
            _fileRepository.SaveItem(file);
        }
    }
}