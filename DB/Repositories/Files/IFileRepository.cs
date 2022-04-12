using DB.Entities;

namespace DB.Repositories.Files;

public interface IFileRepository
{
    public bool SaveItem(FileEntity? item);

    public FileEntity? GetItem(uint id);

    public List<FileEntity> GetItems(string? name = null, string? path = null, uint computerID = 0, string? orderBy = null,
        bool desc = false, uint skip = 0, uint take = 0);

    public uint GetItemsCount(string? name, string? path, uint computerID = 0);
}
