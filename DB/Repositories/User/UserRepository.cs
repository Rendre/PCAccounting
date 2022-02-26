namespace DB.Repositories.User;
using Entities;

public class UserRepository : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public User? GetItem(int id)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
            var user = _databaseContext.GetUser(sqlExpression);
            return user;
    }

    public List<User> GetItems()
    {
        var sqlExpression = $"SELECT * FROM Users WHERE IsDeleted = 0";
        var users = _databaseContext.GetUsers(sqlExpression);
        return users;

    }

    public User? GetItem(string login)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE Login = '{login}' AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetUser(sqlExpression);
        return user;
    }

    public int DeleteItem(int id)
    {
        var sqlExpression = $"UPDATE users SET IsDeleted = 1 WHERE ID = {id}";
        var result = _databaseContext.ExecuteExp(sqlExpression);
        return result;

    }

    public int CreateUser(string login, string password, int employerId)
    {
        var sqlExpression = $"INSERT INTO users (Login, Pass, EmployerId)" +
                            $"VALUES ('{login}', '{password}', {employerId})";
        var sqlExpressionForId = $"SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        return id;
    }

    public int ChangeUser(int id, string login, string password, int employerID)
    {
        var sqlExpression = $"UPDATE users SET Login = '{login}', " +
                            $"Pass = '{password}', EmployerId = {employerID} " +
                            $"WHERE ID = {id}";
        var success = _databaseContext.ExecuteExp(sqlExpression);
        return success;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
