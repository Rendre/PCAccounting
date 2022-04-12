
using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories.Files;

public class FileEFRepository : IFileRepository
{
    private readonly ApplicationContextEF _db;

    public FileEFRepository()
    {
        var opt = new DbContextOptionsBuilder<ApplicationContextEF>();
        opt.UseMySql(MySQLDatabaseContext.ConnectionString, ApplicationContextEF.ServerVersion);
        _db = new ApplicationContextEF(opt.Options);
    }

    public bool SaveItem(FileEntity? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public FileEntity? GetItem(uint id)
    {
        if (id == 0) return null;

        var item = _db.File.FirstOrDefault(p => p.ID == id && p.IsDeleted == false);
        return item;
    }

    public List<FileEntity> GetItems(string? name = null, string? path = null, uint computerID = 0, string? orderBy = null,
        bool desc = false, uint skip = 0, uint take = 0)
    {
        var items = _db.File.Where(p => p.IsDeleted == false);

        if (!string.IsNullOrEmpty(name))
        {
            items = items.Where(p => p.FileName != null && p.FileName.Equals(name));
        }

        if (!string.IsNullOrEmpty(path))
        {
            items = items.Where(p => p.Path != null && p.Path.Equals(path));
        }

        if (computerID > 0)
        {
            items = items.Where(p => p.ComputerID == computerID);
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            // возможно не работает
            var type = typeof(FileEntity);
            var property = type.GetProperty(orderBy);
            // desc = false - возрастание
            items = desc ? items.OrderByDescending(p => property) : items.OrderBy(p => property);
        }

        if (take > 0)
        {
            items = items.Skip((int)skip).Take((int)take);
        }

        return items.ToList();
    }

    public int GetItemsCount(string? name = null, string? path = null, uint computerID = 0)
    {
        var items = _db.File.Where(p => p.IsDeleted == false);

        if (!string.IsNullOrEmpty(name))
        {
            items = items.Where(p => p.FileName != null && p.FileName.Equals(name));
        }

        if (!string.IsNullOrEmpty(path))
        {
            items = items.Where(p => p.Path != null && p.Path.Equals(path));
        }

        if (computerID > 0)
        {
            items = items.Where(p => p.ComputerID == computerID);
        }

        return items.Count();
    }

    private bool CreateItem(FileEntity item)
    {
        _db.File.Add(item);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }

    private bool UpdateItem(FileEntity item)
    {
        _db.File.Update(item);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }
}