namespace DB.Repositories.Computer;
using Entities;

public interface IComputerRepository : IDisposable
{
    public void CreateComputer(Computer computer);

    public dynamic ChangeComputer(Computer computer);

    public Computer? GetComputer(uint id);

    public List<Computer> GetComputers();

    public List<Computer> GetFilterComputers(string name = null, uint statusId = 0, uint employerId = 0,
        DateTime? date = null, string cpu = null, decimal price = 0);

    public dynamic DeleteComputer(uint id);
}