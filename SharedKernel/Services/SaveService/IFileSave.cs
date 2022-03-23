using DB.Entities;

namespace SharedKernel.Services;

public interface IFileSave
{
    public void SaveItem(uint computerID, byte[] fileBytes, string filePath, string pathForSaveFile,
        string fileID ,out FileEntity? file);
}