using DB.Entities;

namespace DB.Repositories.Computers;

public interface IComputerRepository : IDisposable
{
    public bool SaveItem(Computer item);

    public Computer? GetItem(uint id);

    public List<Computer> GetItems(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0, uint skip = 0, uint take = 0);

    public uint GetItemsCount(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0);

}