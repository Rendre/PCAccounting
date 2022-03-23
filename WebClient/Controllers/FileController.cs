﻿using DB.Entities;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Services;
using WebClient.Models;

namespace WebClient.Controllers;

[Route("[controller]")]
public class FileController : Controller
{
    private readonly IFileSave _fileSave = new WebSave();

    [HttpPost]
    public string CreateFile()
    {
        var responceObj = new ResponceObject<FileEntity>();
        string responceJson;

        FileEntity? file;
        try
        {
            byte[]? fileBytes;
            var fileID = "";
            string fileName;
            var computerID = Convert.ToUInt32(HttpContext.Request.Form["computerID"]);
            if (computerID <= 0)
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var fileFromForm = HttpContext.Request.Form.Files["file"];
                if (fileFromForm == null)
                {
                    responceJson = Utils.Util.SerializeToJson(responceObj);
                    return responceJson;
                }

                var ms = new MemoryStream();
                fileFromForm.CopyTo(ms);
                fileBytes = ms.ToArray();
                fileName = fileFromForm.FileName;
            }
            else
            {
                HttpContext.Request.Form.TryGetValue("fileByString", out var strFile);
                fileName = HttpContext.Request.Form["fileName"];
                fileID = HttpContext.Request.Form["fileID"];
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(strFile))
                {
                    responceJson = Utils.Util.SerializeToJson(responceObj);
                    return responceJson;
                }

                fileBytes = Convert.FromBase64String(strFile);
            }

            var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent;
            var pathForSaveFile = directory + "\\Images\\";
            _fileSave.SaveItem(computerID, fileBytes, fileName, pathForSaveFile, fileID, out file);
            if (file.ID > 0)
            {
                responceObj.Data = new FileEntity
                {
                    ComputerID = file.ComputerID,
                    ID = file.ID
                };
                responceObj.Success = 1;
            }

            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;

        }
        catch (Exception)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }
    }
}



