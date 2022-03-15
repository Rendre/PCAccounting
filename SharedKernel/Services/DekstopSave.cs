using DB.Entities;
using DB.Repositories.Picture;

namespace SharedKernel.Services;

public class DekstopSave : IPictureSave
{
    private readonly IPictureRepository _pictureRepository = new PictureDapperRepository();

    public void SaveItem(uint computerId, byte[] pictureBytes, string filePath, string pathForSavePicture, out Picture picture)
    {
        var fileName = Path.GetFileName(filePath);
        var ext = Path.GetExtension(filePath);
        var guidFileName = Guid.NewGuid().ToString("N");
        if (!Directory.Exists(pathForSavePicture))
        {
            Directory.CreateDirectory(pathForSavePicture);
        }
        pathForSavePicture = pathForSavePicture + guidFileName + ext;

        using var fileStream = new FileStream(pathForSavePicture, FileMode.CreateNew);
        fileStream.Write(pictureBytes);
        picture = new Picture() { ComputerId = computerId, Name = fileName, Path = pathForSavePicture };
        _pictureRepository.SaveItem(picture);

        //return pictureBytes;

    }

}