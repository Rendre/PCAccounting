using DB.Entities;

namespace DB.Repositories.File;

public interface IFileRepository
{
    public void SaveItem(Files? file);

    public Files? GetItem(uint id);

    public List<Files?> GetItems(uint computerId, string? orderBy, bool desc, uint limitSkip, uint limitTake);
    public uint DeleteItem(uint id);
}