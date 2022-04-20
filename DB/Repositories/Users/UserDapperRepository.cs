using System.Text;
using Dapper;
using DB.Entities;

namespace DB.Repositories.Users;

public class UserDapperRepository : IUserRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserDapperRepository(MySQLDatabaseContext mySQLDatabaseContext)
    {
        _databaseContext = mySQLDatabaseContext;
    }

    public bool SaveItem(User? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public User? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameter = new DynamicParameters();
        parameter.Add("@ID", id);
        const string sqlExpression = "SELECT * FROM Users WHERE ID = @ID AND IsDeleted = 0 LIMIT 1";
        var item = _databaseContext.GetByQuery<User>(sqlExpression, parameter);
        return item;
    }

    public List<User> GetItems(string? login = null, string? email = null, bool search = false, uint employerID = 0,
        EntityStatus isActivated = EntityStatus.None, string? activationCode = null, uint skip = 0, uint take = 0)
    {
        var sqlExpression = new StringBuilder("SELECT * FROM users WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, parameters, login, email, search, employerID,
            isActivated, activationCode, skip, take);
        var items = _databaseContext.GetAllByQuery<User>(sqlExpressionForQuery, parameters);
        return items;
    }

    public uint GetItemsCount(string? login = null, string? email = null, bool search = false, uint employerID = 0, EntityStatus isActivated = EntityStatus.None,
        string? activationCode = null)
    {
        var sqlExpression = new StringBuilder("SELECT COUNT(*) FROM users WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, parameters, login, email, search, employerID,
            isActivated, activationCode);
        var itemsCount = _databaseContext.ExecuteByQuery(sqlExpressionForQuery, parameters);
        return itemsCount;
    }

    private static string GetParamForExpression(StringBuilder sqlExpression, DynamicParameters parameters, string? login, string? email,
        bool search, uint employerID, EntityStatus isActivated, string? activationCode, uint skip = 0, uint take = 0)
    {
        if (search)
        {
            sqlExpression.Append(" AND (Login=@Login OR Email=@Email)");
            parameters.Add("@Login", login);
            parameters.Add("@Email", email);
        }
        else
        {
            if (!string.IsNullOrEmpty(login))
            {
                sqlExpression.Append(" AND Login = @Login");
                parameters.Add("@Login", login);
            }

            if (!string.IsNullOrEmpty(email))
            {
                sqlExpression.Append(" AND Email = @Email");
                parameters.Add("@Email", email);
            }
        }

        if (employerID > 0)
        {
            sqlExpression.Append(" AND EmployerID = @EmployerID");
            parameters.Add("@EmployerID", employerID);
        }


        if (isActivated != EntityStatus.None)
        {
            sqlExpression.Append(" AND IsActivated = @IsActivated");
            parameters.Add("@IsActivated", isActivated == EntityStatus.OnlyActive ? 1 : 0);
        }

        if (!string.IsNullOrEmpty(activationCode))
        {
            sqlExpression.Append(" AND ActivationCode = @ActivationCode");
            parameters.Add("@ActivationCode", activationCode);
        }

        if (take > 0)
        {
            sqlExpression.Append(" LIMIT @take");
            parameters.Add("@take", take);

            sqlExpression.Append(" OFFSET @skip");
            parameters.Add("@skip", skip);
        }

        return sqlExpression.ToString();
    }

    private bool CreateItem(User user)
    {
        var sqlExpression = "INSERT INTO users (Login, Password, EmployerId, ActivationCode, Email)" +
                            $"VALUES ('{user.Login}', '{user.Password}', {user.EmployerID}, '{user.ActivationCode}', '{user.Email}')";
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForID);
        user.ID = id;
        return id > 0;
    }

    private bool UpdateItem(User user)
    {
        var sqlExpression = "UPDATE users SET " +
                            $"Login = '{user.Login}', " +
                            $"IsDeleted = {user.IsDeleted}, " +
                            $"Password = '{user.Password}', " +
                            $"EmployerID = {user.EmployerID}," +
                            $"IsActivated = {user.IsActivated}, " +
                            $"ActivationCode = '{user.ActivationCode}', " +
                            $"Email = '{user.Email}'" +
                            $"WHERE ID = {user.ID}";
        var countOfChanges = _databaseContext.ExecuteByQuery(sqlExpression);
        return countOfChanges > 0;
    }
}