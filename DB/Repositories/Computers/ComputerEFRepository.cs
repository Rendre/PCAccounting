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

        var computer = _db.Computers.FirstOrDefault(p => p.ID == id && p.IsDeleted == false);
        return computer;
    }

    public List<Computer> GetItems(string? name = null, uint statusID = 0, uint employerID = 0, DateTime? date = null,
        string? cpu = null, decimal price = 0)
    {
        var items = GetList(name, statusID, employerID, date, cpu, price);
        return items;
    }

    public int GetItemsCount(string? name = null, uint statusID = 0, uint employerID = 0, DateTime? date = null,
        string? cpu = null, decimal price = 0)
    {
        var items = GetList(name, statusID, employerID, date, cpu, price);
        return items.Count();
    }

    private List<Computer> GetList(string? name = null, uint statusID = 0, uint employerID = 0, DateTime? date = null,
        string? cpu = null, decimal price = 0)
    {
        var computers = _db.Computers.Where(p => p.IsDeleted == false);
        if (!string.IsNullOrEmpty(name))
        {
            computers = computers.Where(p => p.Name != null && p.Name.Equals(name));
        }

        if (statusID > 0)
        {
            computers = computers.Where(p => p.StatusID == statusID);
        }

        if (employerID > 0)
        {
            computers = computers.Where(p => p.EmployerID == employerID);
        }

        if (date != null)
        {
            computers = computers.Where(p => p.DateCreated.Equals(date));
        }

        if (!string.IsNullOrEmpty(cpu))
        {
            computers = computers.Where(p => p.Cpu != null && p.Cpu.Equals(cpu));
        }

        if (price > 0)
        {
            computers = computers.Where(p => p.Price == price);
        }

        return computers.ToList();
    }

    private bool CreateItem(Computer computer)
    {
        _db.Computers.Add(computer);
        var stateCount = _db.SaveChanges();
        return stateCount > 0;
    }

    private bool UpdateItem(Computer computer)
    {
        _db.Computers.Update(computer);
        var stateCount = _db.SaveChanges();
        return stateCount > 0;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}