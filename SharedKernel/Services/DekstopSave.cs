using DB.Entities;
using DB.Repositories.Picture;

namespace SharedKernel.Services;

public class DekstopSave : IPictureSave
{
    private readonly IPictureRepository _pictureRepository = new PictureRepository();
    
    public dynamic SaveItem(uint computerId, string filePath, string pathForSavePicture, out Picture picture)
    {
        var pictureBytes = File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);
        var ext = Path.GetExtension(filePath);
        var guidFileName = Guid.NewGuid().ToString("N"); 
        
        if (!Directory.Exists(pathForSavePicture))
        {
            Directory.CreateDirectory(pathForSavePicture);
        }

        pathForSavePicture = pathForSavePicture + guidFileName + ext;

        try
        {
            using (FileStream fileStream = new FileStream(pathForSavePicture, FileMode.CreateNew))
            {
                fileStream.Write(pictureBytes);
                picture = new Picture() {ComputerId = computerId,Name = fileName, Path = pathForSavePicture};
                _pictureRepository.SaveItem(picture);

                return pictureBytes;
            }
        }
        catch (Exception ex)
        {
            picture = null;
            return ex;
        }
    }
}