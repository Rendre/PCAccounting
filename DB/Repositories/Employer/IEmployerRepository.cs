namespace DB.Repositories.Employer;
using Entities;

public interface IEmployerRepository : IDisposable
{
    public Employer? GetItem(int id);

    public List<Employer> GetItems();

    public int DeleteItem(int id);

    public int CreateEmployer(string? name, string? position, string? tel);

    public int СhangeEmployer(int id, string? name, string? position, string? tel);
}