using DB.Entities;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Services;
namespace WebClient.Controllers;

[Route("[controller]")]
public class PictureController : Controller
{
    private readonly IPictureSave _pictureSave = new WebSave();

    [HttpPost]
    public dynamic UploadPicture()
    {
        var responseErrObj = new
        {
            success = 0,
            pictureID = 0
        };

        Picture? picture;
        try
        {
            byte[]? pictureBytes;
            var pictureID = "";
            string fileName;
            var computerId = Convert.ToUInt32(HttpContext.Request.Form["computerId"]);
            if (computerId <= 0) return responseErrObj;

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var pic = HttpContext.Request.Form.Files["picture"];
                if (pic == null) return responseErrObj;

                var ms = new MemoryStream();
                pic.CopyTo(ms);
                pictureBytes = ms.ToArray();
                fileName = pic.FileName;
            }
            else
            {
                HttpContext.Request.Form.TryGetValue("pictureByString", out var strImage);
                fileName = HttpContext.Request.Form["fileName"];
                pictureID = HttpContext.Request.Form["pictureId"];
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(strImage)) return responseErrObj;

                pictureBytes = Convert.FromBase64String(strImage);
            }

            var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent;
            var pathForSavePicture = directory + "\\Images\\";
            _pictureSave.SaveItem(computerId, pictureBytes, fileName, pathForSavePicture, pictureID,out picture);

            var resultObj = new
            {
                success = 1,
                pictureID = picture.ID
            };

            return picture.ID == 0 ? responseErrObj : resultObj;
        }
        catch (Exception)
        {
            return responseErrObj;
        }
    }
}



