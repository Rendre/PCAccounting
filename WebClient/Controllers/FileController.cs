using DB.Entities;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Services;
namespace WebClient.Controllers;

[Route("[controller]")]
public class FileController : Controller
{
    private readonly IFileSave _fileSave = new WebSave();

    [HttpPost]
    public dynamic UploadFile()
    {
        var responseErrObj = new
        {
            success = 0,
            pictureID = 0
        };

        Files? file;
        try
        {
            byte[]? fileBytes;
            var fileID = "";
            string fileName;
            var computerId = Convert.ToUInt32(HttpContext.Request.Form["computerId"]);
            if (computerId <= 0) return responseErrObj;

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var fileFromForm = HttpContext.Request.Form.Files["file"];
                if (fileFromForm == null) return responseErrObj;

                var ms = new MemoryStream();
                fileFromForm.CopyTo(ms);
                fileBytes = ms.ToArray();
                fileName = fileFromForm.FileName;
            }
            else
            {
                HttpContext.Request.Form.TryGetValue("fileByString", out var strFile);
                fileName = HttpContext.Request.Form["fileName"];
                fileID = HttpContext.Request.Form["fileId"];
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(strFile)) return responseErrObj;

                fileBytes = Convert.FromBase64String(strFile);
            }

            var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent;
            var pathForSaveFile = directory + "\\Images\\";
            _fileSave.SaveItem(computerId, fileBytes, fileName, pathForSaveFile, fileID,out file);

            var resultObj = new
            {
                success = 1,
                fileID = file.ID
            };

            return file.ID == 0 ? responseErrObj : resultObj;
        }
        catch (Exception)
        {
            return responseErrObj;
        }
    }
}



