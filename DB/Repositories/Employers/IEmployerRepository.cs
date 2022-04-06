using DB.Entities;

namespace DB.Repositories.Employers;

public interface IEmployerRepository : IDisposable
{
    public bool SaveItem(Employer item);

    public Employer? GetItem(uint id);

    public List<Employer> GetItems(string? name = null, string? position = null, string? tel = null);

    public int GetItemsCount(string? name = null, string? position = null, string? tel = null);

}