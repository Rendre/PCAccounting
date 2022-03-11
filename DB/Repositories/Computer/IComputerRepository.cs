namespace DB.Repositories.Computer;
using Entities;

public interface IComputerRepository : IDisposable
{
    public void CreateComputer(Computer computer);

    public bool ChangeComputer(Computer computer);

    public Computer? GetComputer(uint id);

    public List<Computer> GetFilterComputers(string? name = null, uint statusId = 0, uint employerId = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0);

    public bool DeleteComputer(uint id);
}