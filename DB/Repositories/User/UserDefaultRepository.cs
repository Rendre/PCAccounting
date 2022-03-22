namespace DB.Repositories.User;

public class UserDefaultRepository : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserDefaultRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void CreateItem(Entities.User user)
    {
        var sqlExpression = "INSERT INTO users (Login, Pass, EmployerId)" +
                            $"VALUES ('{user.Login}', '{user.Pass}', {user.EmployerId})";
        const string sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        user.ID = id;
    }

    public bool ChangeItem(uint id, string? login, string? password, uint employerID)
    {
        var sqlExpression = $"UPDATE users SET Login = '{login}', " +
                            $"Pass = '{password}', EmployerId = {employerID} " +
                            $"WHERE ID = {id}";
        var success = _databaseContext.ExecuteExp(sqlExpression);
        return success > 0;
    }

    public Entities.User? GetItem(uint id)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
            var user = _databaseContext.GetUser(sqlExpression);
            return user;
    }

    public List<Entities.User> GetItems()
    {
        const string sqlExpression = "SELECT * FROM Users WHERE IsDeleted = 0";
        var users = _databaseContext.GetUsers(sqlExpression);
        return users;

    }

    public Entities.User? GetItem(string? login)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE Login = '{login}' AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetUser(sqlExpression);
        return user;
    }

    public uint DeleteItem(uint id)
    {
        var sqlExpression = $"UPDATE users SET IsDeleted = 1 WHERE ID = {id}";
        var result = _databaseContext.ExecuteExp(sqlExpression);
        return result;

    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
