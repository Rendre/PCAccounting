namespace DB.Repositories.Employer;
using Entities;

public interface IEmployerRepository : IDisposable
{
    public void CreateItem(Employer employer);

    public bool СhangeItem(Employer employer);

    public Employer? GetItem(uint id);

    public List<Employer> GetItems();

    public List<Employer> GetItems(string? name, string? position, string? tel);

    public uint DeleteItem(uint id);

}