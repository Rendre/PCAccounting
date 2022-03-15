using DB.Entities;

namespace SharedKernel.Services;

public interface IPictureSave
{
    public void SaveItem(uint computerId, byte[] pictureBytes, string filePath, string pathForSavePicture,
        out Picture picture);
}