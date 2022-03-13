namespace DB.Repositories.User;
using Entities;
using Dapper;

public class UserDapperRepository : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserDapperRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }
    public void CreateItem(User user)
    {
        var sqlExpression = "INSERT INTO users (Login, Pass, EmployerId)" +
                            $"VALUES ('{user.Login}', '{user.Pass}', {user.EmployerId})";
        const string sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
        user.ID = id;
    }

    public bool ChangeItem(uint id, string? login, string? password, uint employerID)
    {
        var sqlExpression = $"UPDATE users SET Login = '{login}', " +
                            $"Pass = '{password}', EmployerId = {employerID} " +
                            $"WHERE ID = {id}";
        var success = _databaseContext.ExecuteByQuery(sqlExpression);
        return success > 0;
    }

    public User? GetItem(uint id)
    {
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
        var parameters = new DynamicParameters();
        parameters.Add("@Login", login);
        const string sqlExpression = "SELECT * FROM Users WHERE Login = @Login AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetByQuery<User>(sqlExpression, parameters);
        return user;
    }

    public uint DeleteItem(uint id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        const string sqlExpression = "UPDATE users SET IsDeleted = 1 WHERE ID = @ID";
        var result = _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        return result;

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