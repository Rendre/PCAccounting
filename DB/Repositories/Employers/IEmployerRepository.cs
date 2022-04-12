using DB.Entities;

namespace DB.Repositories.Employers;

public interface IEmployerRepository : IDisposable
{
    public bool SaveItem(Employer item);

    public Employer? GetItem(uint id);

    public List<Employer> GetItems(string? name = null, string? position = null, string? tel = null,
        uint skip = 0, uint take = 0);

    public uint GetItemsCount(string? name = null, string? position = null, string? tel = null);

}