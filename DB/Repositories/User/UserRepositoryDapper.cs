using Dapper;

namespace DB.Repositories.User;

public class UserRepositoryDapper : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserRepositoryDapper()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public Entities.User GetItem(uint id)
    {
        var parameter = new DynamicParameters();
        parameter.Add("@ID", id);
        var sqlExpression = "SELECT * FROM Users WHERE ID = @ID AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetByQuery<Entities.User>(sqlExpression, parameter);
        return user;
    }

    public List<Entities.User> GetItems()
    {
        var sqlExpression = "SELECT * FROM Users WHERE IsDeleted = 0";
        var users = _databaseContext.GetAllByQuery<Entities.User>(sqlExpression);
        return users;
    }

    public Entities.User GetItem(string login)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Login", login);
        var sqlExpression = "SELECT * FROM Users WHERE Login = @Login AND IsDeleted = 0 LIMIT 1";
        var user = _databaseContext.GetByQuery<Entities.User>(sqlExpression, parameters);
        return user;
    }

    public uint DeleteItem(uint id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        var sqlExpression = "UPDATE users SET IsDeleted = 1 WHERE ID = @ID";
        var result = _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        return result;

    }

    public uint CreateUser(string login, string password, uint employerId)
    {
        var sqlExpression = "INSERT INTO users (Login, Pass, EmployerId)" +
                            $"VALUES ('{login}', '{password}', {employerId})";
        var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
        return id;
    }

    public uint ChangeUser(uint id, string login, string password, uint employerID)
    {
        var sqlExpression = $"UPDATE users SET Login = '{login}', " +
                            $"Pass = '{password}', EmployerId = {employerID} " +
                            $"WHERE ID = {id}";
        var success = _databaseContext.ExecuteByQuery(sqlExpression);
        return success;
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