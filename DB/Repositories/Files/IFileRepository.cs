using DB.Entities;

namespace DB.Repositories.Files;

public interface IFileRepository
{
    public void SaveItem(FileEntity? file);

    public FileEntity? GetItem(uint id);

    public List<FileEntity?> GetItems(uint computerId, string? orderBy, bool desc, uint limitSkip, uint limitTake);
    public uint DeleteItem(uint id);
}