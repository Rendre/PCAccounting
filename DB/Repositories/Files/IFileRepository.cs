using DB.Entities;

namespace DB.Repositories.Files;

public interface IFileRepository
{
    public void CreateItem(FileEntity? file);

    public FileEntity? GetItem(uint id);

    public List<FileEntity?> GetItems(uint computerID = 0, string? orderBy = null, bool desc = false, uint limitSkip = 0, uint limitTake = 0);

    public uint DeleteItem(uint id);
}