namespace DB.Repositories.Computer;

public interface IComputerRepository : IDisposable
{
    public void CreateItem(Entities.Computer computer);

    public bool ChangeItem(Entities.Computer computer);

    public Entities.Computer? GetItem(uint id);

    public List<Entities.Computer> GetFilterItems(string? name = null, uint statusId = 0, uint employerId = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0);

    public bool DeleteItem(uint id);
}