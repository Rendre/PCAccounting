namespace DB.Repositories.Session;

public class SessionEFRepository : ISessionRepository
{

    private readonly MySQLDatabaseContext _databaseContext;

    public SessionEFRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }
}