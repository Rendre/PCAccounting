using DB.Entities;

namespace DB.Repositories.Employers;

public class EmployerEFRepository : IEmployerRepository
{
    private readonly ApplicationContextEF _db;
    public EmployerEFRepository(ApplicationContextEF applicationContextEF)
    {
        _db = applicationContextEF;
    }

    public bool SaveItem(Employer? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public Employer? GetItem(uint id)
    {
        if (id == 0) return null;

        var item = _db.Employers.FirstOrDefault(p => p.ID == id && p.IsDeleted == false);
        return item;
    }

    public List<Employer> GetItems(string? name = null, string? position = null, string? tel = null,
        uint skip = 0, uint take = 0)
    {
        var items = GetList(name, position, tel, skip, take);
        return items.ToList();
    }

    public uint GetItemsCount(string? name = null, string? position = null, string? tel = null)
    {
        var items = GetList(name, position, tel);
        return (uint)items.Count();
    }

    private IQueryable<Employer> GetList(string? name = null, string? position = null, string? tel = null,
                                            uint skip = 0, uint take = 0)
    {
        var items = _db.Employers.Where(p => p.IsDeleted == false);

        if (!string.IsNullOrEmpty(name))
        {
            items = items.Where(p => p.Name != null && p.Name.Equals(name));
        }

        if (!string.IsNullOrEmpty(position))
        {
            items = items.Where(p => p.Position != null && p.Position.Equals(position));
        }

        if (!string.IsNullOrEmpty(tel))
        {
            items = items.Where(p => p.Tel != null && p.Tel.Equals(tel));
        }

        if (take > 0)
        {
            items = items.Skip((int)skip).Take((int)take);
        }

        return items;
    }

    private bool CreateItem(Employer item)
    {
        _db.Employers.Add(item);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }

    private bool UpdateItem(Employer item)
    {
        _db.Employers.Update(item);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }
}