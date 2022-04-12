using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories.Computers;

public class ComputerEFRepository : IComputerRepository
{
    private readonly ApplicationContextEF _db;

    public ComputerEFRepository()
    {
        var opt = new DbContextOptionsBuilder<ApplicationContextEF>();
        opt.UseMySql(MySQLDatabaseContext.ConnectionString, ApplicationContextEF.ServerVersion);
        _db = new ApplicationContextEF(opt.Options);
    }

    public bool SaveItem(Computer? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public Computer? GetItem(uint id)
    {
        if (id == 0) return null;

        var item = _db.Computers.FirstOrDefault(p => p.ID == id && p.IsDeleted == false);
        return item;
    }

    public List<Computer> GetItems(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0, uint skip = 0, uint take = 0)
    {
        var items = GetList(name, date, statusID, employerID, cpu, price);
        return items;
    }

    public uint GetItemsCount(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0)
    {
        var items = GetList(name, date, statusID, employerID, cpu, price);
        return (uint)items.Count();
    }

    private List<Computer> GetList(string? name = null, DateTime? date = null, uint statusID = 0, uint employerID = 0,
        string? cpu = null, decimal price = 0, uint skip = 0, uint take = 0)
    {
        var items = _db.Computers.Where(p => p.IsDeleted == false);
        if (!string.IsNullOrEmpty(name))
        {
            items = items.Where(p => p.Name != null && p.Name.Equals(name));
        }

        if (statusID > 0)
        {
            items = items.Where(p => p.StatusID == statusID);
        }

        if (employerID > 0)
        {
            items = items.Where(p => p.EmployerID == employerID);
        }

        if (date != null)
        {
            items = items.Where(p => p.DateCreated.Equals(date));
        }

        if (!string.IsNullOrEmpty(cpu))
        {
            items = items.Where(p => p.Cpu != null && p.Cpu.Equals(cpu));
        }

        if (price > 0)
        {
            items = items.Where(p => p.Price == price);
        }

        if (take > 0)
        {
            items = items.Skip((int)skip).Take((int)take);
        }

        return items.ToList();
    }

    private bool CreateItem(Computer computer)
    {
        _db.Computers.Add(computer);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }

    private bool UpdateItem(Computer computer)
    {
        _db.Computers.Update(computer);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}