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
        _db.Technick.Add(computer);
        _db.SaveChanges();
    }

    public bool ChangeItem(Computer computer)
    {
        _db.Technick.Update(computer);
        var rowsChanges = _db.SaveChanges();
        return rowsChanges > 0;
    }

    public Computer? GetItem(uint id)
    {
        if (id == 0) return null;

        var computer = _db.Technick.FirstOrDefault(p => p.ID == id);
        return computer;
    }

    public List<Computer> GetFilterItems(string? name = null, uint statusId = 0, uint employerId = 0, DateTime? date = null,
        string? cpu = null, decimal price = 0)
    {
        var computers = _db.Technick.Where(p => p.IsDeleted == false);
        if (!string.IsNullOrEmpty(name))
        {
            computers = computers.Where(p => p.Name != null && p.Name.Equals(name));
        }

        if (statusId > 0)
        {
            computers = computers.Where(p => p.StatusID == statusId);
        }

        if (employerId > 0)
        {
            computers = computers.Where(p => p.EmployerId == employerId);
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
        var computer = _db.Technick.FirstOrDefault(p => p.ID == id);
        if (computer == null) return false;

        computer.IsDeleted = true;
        _db.Technick.Update(computer);
        var rowsChanges = _db.SaveChanges();
        return rowsChanges > 0;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}