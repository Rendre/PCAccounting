using DB.Entities;
using DB.Repositories.Picture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SharedKernel.Services;

namespace WebClient.Controllers;

[Route("[controller]")]
public class PictureController : Controller
{
    private readonly IPictureSave _pictureSave = new WebSave();
    private readonly IWebHostEnvironment _appEnvironment;


    public PictureController(IWebHostEnvironment appEnvironment)
    {
        _appEnvironment = appEnvironment;
    }

    [HttpPost]
    public dynamic UploadPicture()
    {
        Picture picture;
        try
        {

            var pic = HttpContext.Request.Form.Files["picture"];
            var computerId = Convert.ToUInt32(HttpContext.Request.Form["computerId"]);
            if (pic == null) return null;

            var ms = new MemoryStream();
            pic.CopyTo(ms);
            var pictureBytes = ms.ToArray();

            var fileName = pic.FileName;
            var directory = Environment.CurrentDirectory;
            var pathForSavePicture = directory + "/../Images/";

            _pictureSave.SaveItem(computerId, pictureBytes, fileName, pathForSavePicture, out picture);
        }
        catch (Exception)
        {
            return 0;
        }
        return picture.ID == 0 ? 0 : 1;
    }
}



