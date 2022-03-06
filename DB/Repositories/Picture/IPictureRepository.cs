namespace DB.Repositories.Picture;
using Entities;

public interface IPictureRepository
{
    public void SaveItem(Picture picture);

    public Picture? GetItem(uint id);

    public List<Picture> GetItems(uint computerId, string? orderBy, bool desc, int limitSkip, int limitTake);
    public uint DeleteItem(uint id);
}