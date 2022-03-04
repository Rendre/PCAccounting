namespace DB.Repositories.Picture;
using Entities;

public interface IPictureRepository
{
    public void SaveItem(Picture picture);
}