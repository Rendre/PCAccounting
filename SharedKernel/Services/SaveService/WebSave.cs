﻿using DB.Entities;
using DB.Repositories.File;

namespace SharedKernel.Services;

public class WebSave : IFileSave
{
    private readonly IFileRepository _fileRepository = new FileDapperRepository();
    //todo:
    public void SaveItem(uint computerId, byte[] fileBytes, string fileName, string pathForSaveFile, string fileID,out Files? file)
    {
        if (string.IsNullOrEmpty(fileID))
        {
            file = null;
            var guidFileName = Guid.NewGuid().ToString("N");
            var extension = Path.GetExtension(fileName);
            if (!Directory.Exists(pathForSaveFile))
            {
                Directory.CreateDirectory(pathForSaveFile);
            }

            pathForSaveFile = pathForSaveFile + guidFileName + extension;
        }
        else
        {
            var kekid = Convert.ToUInt32(fileID);
            file = _fileRepository.GetItem(kekid);
            if (file == null) return;

            pathForSaveFile = file.Path!;
        }

        using var fileStream = new FileStream(pathForSaveFile, FileMode.Append, FileAccess.Write);
        fileStream.Write(fileBytes);
        if (file == null)
        {
            file = new Files() { ComputerId = computerId, Name = fileName, Path = pathForSaveFile };
            _fileRepository.SaveItem(file);
        }
    }
}