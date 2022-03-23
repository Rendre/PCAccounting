using DB.Entities;

namespace DB.Repositories.Computers;

public interface IComputerRepository : IDisposable
{
    public void CreateItem(Computer computer);

    public bool UpdateItem(Computer computer);

    public Computer? GetItem(uint id);

    public List<Computer> GetFilterItems(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0);

    public bool DeleteItem(uint id);
}