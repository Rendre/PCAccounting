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

    public void CreateItem(Computer computer)
    {
        _db.Computers.Add(computer);
        _db.SaveChanges();
    }

    public bool UpdateItem(Computer computer)
    {
        _db.Computers.Update(computer);
        var rowsChanges = _db.SaveChanges();
        return rowsChanges > 0;
    }

    public Computer? GetItem(uint id)
    {
        if (id == 0) return null;

        var computer = _db.Computers.FirstOrDefault(p => p.ID == id && p.IsDeleted == false);
        return computer;
    }

    public List<Computer> GetFilterItems(string? name = null, uint statusID = 0, uint employerID = 0, DateTime? date = null,
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

    public bool DeleteItem(uint id)
    {
        var computer = _db.Computers.FirstOrDefault(p => p.ID == id);
        if (computer == null) return false;

        computer.IsDeleted = true;
        _db.Computers.Update(computer);
        var rowsChanges = _db.SaveChanges();
        return rowsChanges > 0;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}