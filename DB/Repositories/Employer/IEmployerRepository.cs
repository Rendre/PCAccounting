namespace DB.Repositories.Employer;
using Entities;

public interface IEmployerRepository : IDisposable
{
    public Employer? GetItem(uint id);

    public List<Employer> GetItems();

    public List<Employer> GetItems(string? name, string? position, string? tel);

    public uint DeleteItem(uint id);

    public uint CreateEmployer(Employer employer);

    public uint СhangeEmployer(Employer employer);
}