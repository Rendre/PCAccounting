namespace DB.Repositories.Computer;
using Entities;

public interface IComputerRepository : IDisposable
{
    public void CreateItem(Computer computer);

    public bool ChangeItem(Computer computer);

    public Computer? GetItem(uint id);

    public List<Computer> GetFilterItems(string? name = null, uint statusId = 0, uint employerId = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0);

    public bool DeleteItem(uint id);
}