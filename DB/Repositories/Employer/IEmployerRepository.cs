namespace DB.Repositories.Employer;

public interface IEmployerRepository : IDisposable
{
    public void CreateItem(Entities.Employer employer);

    public bool ChangeItem(Entities.Employer employer);

    public Entities.Employer? GetItem(uint id);

    public List<Entities.Employer> GetItems();

    public List<Entities.Employer> GetItems(string? name, string? position, string? tel);

    public uint DeleteItem(uint id);

}