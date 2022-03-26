using DB.Entities;

namespace DB.Repositories.Users;

public class UserDefaultRepository : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserDefaultRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public bool CreateItem(User user)
    {
        var sqlExpression = "INSERT INTO users (Login, Password, EmployerId, ActivationCode, Email)" +
                            $"VALUES ('{user.Login}', '{user.Password}', {user.EmployerID}," +
                            $"'{user.ActivationCode}', '{user.Email}')";
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForID);
        user.ID = id;
        return id > 0;
    }

    public bool UpdateItem(User user)
    {
        var sqlExpression = $"UPDATE users SET Login = '{user.Login}', " +
                            $"Password = '{user.Password}', EmployerID = {user.EmployerID}," +
                            $"IsActivated = {user.IsActivated} , ActivationCode = '{user.ActivationCode}', Email = '{user.Email}' " +
                            $"WHERE ID = {user.ID}";
        var rowsChanged = _databaseContext.ExecuteExp(sqlExpression);
        return rowsChanged > 0;
    }

    public User? GetItem(uint id)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
            var user = _databaseContext.GetUser(sqlExpression);
            return user;
    }

    public List<User> GetItems()
    {
        const string sqlExpression = "SELECT * FROM Users WHERE IsDeleted = 0";
        var users = _databaseContext.GetUsers(sqlExpression);
        return users;

    }

    public User? GetItem(string? login)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE Login = '{login}' AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetUser(sqlExpression);
        return user;
    }

    public User? GetItemByEmail(string? email)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE Email = '{email}' AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetUser(sqlExpression);
        return user;
    }

    public bool DeleteItem(uint id)
    {
        var sqlExpression = $"UPDATE users SET IsDeleted = 1 WHERE ID = {id}";
        var rowsChanges = _databaseContext.ExecuteExp(sqlExpression);
        return rowsChanges > 0;

    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
