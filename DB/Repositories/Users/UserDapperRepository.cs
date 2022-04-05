using Dapper;
using DB.Entities;

namespace DB.Repositories.Users;

public class UserDapperRepository : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserDapperRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }
    public bool CreateItem(User user)
    {
        var sqlExpression = "INSERT INTO users (Login, Password, EmployerId, ActivationCode, Email)" +
                            $"VALUES ('{user.Login}', '{user.Password}', {user.EmployerID}, '{user.ActivationCode}', '{user.Email}')";
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForID);
        user.ID = id;
        return id > 0;
    }

    public bool UpdateItem(User user)
    {
        var sqlExpression = $"UPDATE users SET Login = '{user.Login}', " +
                            $"Password = '{user.Password}', EmployerID = {user.EmployerID}," +
                            $"IsActivated = {user.IsActivated}, ActivationCode = '{user.ActivationCode}', Email = '{user.Email}'" +
                            $"WHERE ID = {user.ID}";
        var rowsChanged = _databaseContext.ExecuteByQuery(sqlExpression);
        return rowsChanged > 0;
    }

    public User? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameter = new DynamicParameters();
        parameter.Add("@ID", id);
        const string sqlExpression = "SELECT * FROM Users WHERE ID = @ID AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetByQuery<User>(sqlExpression, parameter);
        return user;
    }

    public List<User> GetItems()
    {
        const string sqlExpression = "SELECT * FROM Users WHERE IsDeleted = 0";
        var users = _databaseContext.GetAllByQuery<User>(sqlExpression);
        return users;
    }

    public User? GetItem(string? login)
    {
        if (string.IsNullOrEmpty(login)) return null;

        var parameters = new DynamicParameters();
        parameters.Add("@Login", login);
        const string sqlExpression = "SELECT * FROM Users WHERE Login = @Login AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetByQuery<User>(sqlExpression, parameters);
        return user;
    }


    public User? GetItemByEmail(string? email)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Email", email);
        const string sqlExpression = "SELECT * FROM Users WHERE Email = @Email AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetByQuery<User>(sqlExpression, parameters);
        return user;
    }

    public bool DeleteItem(uint id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        const string sqlExpression = "UPDATE users SET IsDeleted = 1 WHERE ID = @ID";
        var rowsChanges = _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        return rowsChanges > 0;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}

//public User GetItem(uint id)
//{
//    var sqlExpression = $"SELECT * FROM Users WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
//    var user = _databaseContext.GetByQuery<User>(sqlExpression);
//    return user;
//}

//public User GetItem(string login)
//{
//    var sqlExpression = $"SELECT * FROM Users WHERE Login = '{login}' AND IsDeleted = 0 LIMIT 1";
//    var user = _databaseContext.GetByQuery<User>(sqlExpression);
//    return user;
//}