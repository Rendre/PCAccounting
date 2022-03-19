using DB.Entities;
using DB.Repositories.Picture;
namespace SharedKernel.Services;

public class WebSave : IPictureSave
{
    private readonly IPictureRepository _pictureRepository = new PictureDapperRepository();
    //todo:
    public void SaveItem(uint computerId, byte[] pictureBytes, string fileName, string pathForSavePicture, string pictureID,out Picture? picture)
    {
        if (string.IsNullOrEmpty(pictureID))
        {
            picture = null;
            var guidFileName = Guid.NewGuid().ToString("N");
            var extension = Path.GetExtension(fileName);
            if (!Directory.Exists(pathForSavePicture))
            {
                Directory.CreateDirectory(pathForSavePicture);
            }

            pathForSavePicture = pathForSavePicture + guidFileName + extension;
        }
        else
        {
            var kekid = Convert.ToUInt32(pictureID);
            picture = _pictureRepository.GetItem(kekid);
            if (picture == null) return;

            pathForSavePicture = picture.Path!;
        }

        using var fileStream = new FileStream(pathForSavePicture, FileMode.Append, FileAccess.Write);
        fileStream.Write(pictureBytes);
        if (picture == null)
        {
            picture = new Picture() { ComputerId = computerId, Name = fileName, Path = pathForSavePicture };
            _pictureRepository.SaveItem(picture);
        }
    }
}