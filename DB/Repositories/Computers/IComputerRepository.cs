using DB.Entities;

namespace DB.Repositories.Computers;

public interface IComputerRepository : IDisposable
{
    public bool SaveItem(Computer item);

    public Computer? GetItem(uint id);

    public List<Computer> GetItems(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0);

    public int GetItemsCount(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0);

}