namespace DB.Repositories.Computer;
using Entities;

public interface IComputerRepository
{
    public dynamic CreateComputer(string name, int statusId, int employerId, DateTime date,
        string cpu, decimal price);

    public dynamic ChangeComputer(int id, string name, int statusId, int employerId, DateTime date,
        string cpu, decimal price);

    public Computer GetComputer(int id);

    public List<Computer> GetComputers();

    public dynamic DeleteComputer(int id);
}