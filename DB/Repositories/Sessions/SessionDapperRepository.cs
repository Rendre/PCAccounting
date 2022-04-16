using System.Text;
using Dapper;
using DB.Entities;

namespace DB.Repositories.Sessions;

public class SessionDapperRepository : ISessionRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public SessionDapperRepository(MySQLDatabaseContext mySQLDatabaseContext)
    {
        _databaseContext = mySQLDatabaseContext;
    }

    public bool SaveItem(Session? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public Session? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameter = new DynamicParameters();
        parameter.Add("@ID", id);
        const string sqlExpression = "SELECT * FROM session WHERE ID = @ID AND IsDeleted = 0 LIMIT 1";
        var item = _databaseContext.GetByQuery<Session>(sqlExpression, parameter);
        return item;
    }

    public List<Session?> GetItems(string? token = null, DateTime? time = null, uint userID = 0,
        string? userIP = null, uint skip = 0, uint take = 0)
    {
        var sqlExpression = new StringBuilder("SELECT * FROM session WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, parameters, token, time, userID, userIP, skip, take);
        var items = _databaseContext.GetAllByQuery<Session>(sqlExpressionForQuery, parameters);
        return items!;
    }

    public uint GetItemsCount(string? token = null, DateTime? time = null, uint userID = 0,
        string? userIP = null)
    {
        var sqlExpression = new StringBuilder("SELECT COUNT(*) FROM session WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, parameters, token, time, userID, userIP);
        var itemsCount = _databaseContext.ExecuteByQuery(sqlExpressionForQuery, parameters);
        return itemsCount;
    }

    private bool CreateItem(Session item)
    {
        // проверить записывается ли время
        var sqlExpression = "INSERT INTO session (Token, Time, UserID, UserIP)" +
                            $"VALUES ('{item.Token}', '{item.Time:yyyy-MM-dd HH:mm:ss}', {item.UserID}, '{item.UserIP}') ";
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForID);
        item.ID = id;
        return id > 0;
    }

    private bool UpdateItem(Session item)
    {
        var sqlExpression = "UPDATE users SET " +
                            $"Token = '{item.Token}', " +
                            $"Time = '{item.Time:yyyy-MM-dd HH:mm:ss}', " +
                            $"UserID = {item.UserID}, " +
                            $"UserIP = '{item.UserIP}', " +
                            $"IsDeleted = {item.IsDeleted}, " +
                            $"WHERE ID = {item.ID}";
        var countOfChanges = _databaseContext.ExecuteByQuery(sqlExpression);
        return countOfChanges > 0;
    }

    private static string GetParamForExpression(StringBuilder sqlExpression, DynamicParameters parameters,
        string? token, DateTime? time, uint userID, string? userIP, uint skip = 0, uint take = 0)
    {
        if (!string.IsNullOrEmpty(token))
        {
            sqlExpression.Append(" AND Token=@Token");
            parameters.Add("@Token", token);
        }

        if (time != null)
        {
            sqlExpression.Append(" AND Time=@Time");
            parameters.Add("@Time", time);
        }

        if (userID > 0)
        {
            sqlExpression.Append(" AND UserID=@UserID");
            parameters.Add("@UserID", userID);
        }

        if (!string.IsNullOrEmpty(userIP))
        {
            sqlExpression.Append(" AND UserIP=@UserIP");
            parameters.Add("@UserIP", userIP);
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
}