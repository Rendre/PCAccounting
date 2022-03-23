using DB.Entities;

namespace DB.Repositories.Employers;

public interface IEmployerRepository : IDisposable
{
    public void CreateItem(Employer employer);

    public bool UpdateItem(Employer employer);

    public Employer? GetItem(uint id);

    //public List<Employer> GetItems();

    public List<Employer> GetItems(string? name = null, string? position = null, string? tel = null);

    public uint DeleteItem(uint id);

}