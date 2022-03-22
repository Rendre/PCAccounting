namespace DB.Repositories.User;

public interface IUserRepository : IDisposable
{
    public void CreateItem(Entities.User user);

    public bool ChangeItem(uint id, string? login, string? password, uint employerID);

    public Entities.User? GetItem(uint id);

    public List<Entities.User> GetItems();

    public Entities.User? GetItem(string? login);

    public uint DeleteItem(uint id);
}