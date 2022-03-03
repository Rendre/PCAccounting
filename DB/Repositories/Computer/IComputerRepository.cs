namespace DB.Repositories.Computer;
using Entities;

public interface IComputerRepository : IDisposable
{
    public void CreateComputer(Computer computer);

    public dynamic ChangeComputer(Computer computer);

    public Computer? GetComputer(int id);

    public List<Computer> GetComputers();

    public List<Computer> GetFilterComputers(string name = null, int statusId = 0, int employerId = 0,
        DateTime date = default(DateTime),
        string cpu = null, decimal price = 0);

    public dynamic DeleteComputer(int id);
}