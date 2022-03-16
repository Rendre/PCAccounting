using DB.Entities;
using DB.Repositories.Picture;
namespace SharedKernel.Services;

public class WebSave : IPictureSave
{
    private readonly IPictureRepository _pictureRepository = new PictureDapperRepository();
    //todo:
    public void SaveItem(uint computerId, byte[] pictureBytes, string fileName, string pathForSavePicture, out Picture picture)
    {
        var guidFileName = Guid.NewGuid().ToString("N");
        var extension = Path.GetExtension(fileName);
        if (!Directory.Exists(pathForSavePicture))
        {
            Directory.CreateDirectory(pathForSavePicture);
        }
        pathForSavePicture = pathForSavePicture + guidFileName + extension;

        using var fileStream = new FileStream(pathForSavePicture, FileMode.CreateNew);
        fileStream.Write(pictureBytes);
        picture = new Picture() { ComputerId = computerId, Name = fileName, Path = pathForSavePicture };
        _pictureRepository.SaveItem(picture);
    }
}