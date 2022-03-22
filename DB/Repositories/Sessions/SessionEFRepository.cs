namespace DB.Repositories.Sessions;

public class SessionEFRepository : ISessionRepository
{

    private readonly MySQLDatabaseContext _databaseContext;

    public SessionEFRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }
}