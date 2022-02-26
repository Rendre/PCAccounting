namespace DB.Repositories.Computer;
using Entities;

public interface IComputerRepository : IDisposable
{
    public void CreateComputer(Computer computer);

    public dynamic ChangeComputer(Computer computer);

    public Computer? GetComputer(int id);

    public List<Computer> GetComputers();

    public dynamic DeleteComputer(int id);
}