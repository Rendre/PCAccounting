namespace DB.Repositories.User;
using Entities;

public class UserRepositoryDefault : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserRepositoryDefault()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public User? GetItem(uint id)
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

    public uint DeleteItem(uint id)
    {
        var sqlExpression = $"UPDATE users SET IsDeleted = 1 WHERE ID = {id}";
        var result = (uint) _databaseContext.ExecuteExp(sqlExpression);
        return result;

    }

    public uint CreateUser(string login, string password, uint employerId)
    {
        var sqlExpression = $"INSERT INTO users (Login, Pass, EmployerId)" +
                            $"VALUES ('{login}', '{password}', {employerId})";
        var sqlExpressionForId = $"SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        return id;
    }

    public uint ChangeUser(uint id, string login, string password, uint employerID)
    {
        var sqlExpression = $"UPDATE users SET Login = '{login}', " +
                            $"Pass = '{password}', EmployerId = {employerID} " +
                            $"WHERE ID = {id}";
        var success = (uint) _databaseContext.ExecuteExp(sqlExpression);
        return success;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
