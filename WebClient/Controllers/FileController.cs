﻿using System.Text.Json;
using DB.Entities;
using DB.Repositories.Files;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Services;
using SharedKernel.Services.DownloadService;
using WebClient.Models;

namespace WebClient.Controllers;

[Route("[controller]")]
public class FileController : Controller
{
    private readonly IFileSave _fileSave = new WebSave();
    private readonly IFileDownload _fileDownload = new WebDownload();
    private readonly ILogger<FileController> _logger;

    public FileController(ILogger<FileController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public string CreateFile()
    {
        var responceObj = new ResponceObject<FileEntity>();
        string responceJson;

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
            _fileSave.SaveItem(computerID, fileBytes, fileName, pathForSaveFile, fileID, out var file);
            if (file.ID > 0)
            {
                responceObj.Data = file;
                responceObj.Success = 1;
            }

            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }
    }

    [HttpGet]
    public IActionResult GetPucture([FromBody] JsonElement json)
    {
        byte[]? pictureBytes = null;
        string? fileType = null;
        string? fileName = null;

        try
        {
            uint id = 0;
            if (json.TryGetProperty("id", out var idElement))
            {
                id = idElement.GetUInt32();
            }
            if (id == 0) return NotFound();

            _fileDownload.GetItem(id, out pictureBytes, out fileType, out fileName);
            if (pictureBytes == null ||
                fileType == null ||
                fileName == null) return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return File(pictureBytes!, fileType!, fileName!);
    }
}