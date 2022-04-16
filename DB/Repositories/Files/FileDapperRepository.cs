using Dapper;
using DB.Entities;

namespace DB.Repositories.Files;

public class FileDapperRepository : IFileRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public FileDapperRepository(MySQLDatabaseContext mySQLDatabaseContext)
    {
        _databaseContext = mySQLDatabaseContext;
    }

    public bool SaveItem(FileEntity? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public FileEntity? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);

        const string sqlExpression = "SELECT * FROM files WHERE ID = @ID AND IsDeleted = 0";
        var item = _databaseContext.GetByQuery<FileEntity>(sqlExpression, parameters);
        return item;
    }

    public List<FileEntity> GetItems(string? name = null, string? path = null, uint computerID = 0, string? orderBy = null,
        bool desc = false, uint skip = 0, uint take = 0)
    {
        var parameters = new DynamicParameters();
        var conditions = new List<string> { "IsDeleted=0" };

        if (!string.IsNullOrEmpty(name))
        {
            conditions.Add("FileName=@FileName");
            parameters.Add("@FileName", name);
        }

        if (!string.IsNullOrEmpty(path))
        {
            conditions.Add("Path=@Path");
            parameters.Add("@Path", path);
        }

        if (computerID > 0)
        {
            conditions.Add("ComputerID=@ComputerID");
            parameters.Add("@ComputerID", computerID);
        }
        var sqlExpression = $"SELECT * FROM files WHERE {string.Join(" AND ", conditions)}";

        if (!string.IsNullOrEmpty(orderBy))
        {
            sqlExpression += $" ORDER BY {orderBy} {(desc ? "DESC" : "ASC")}";
        }

        if (take > 0)
        {
            sqlExpression += " LIMIT @limitSkip, @limitTake";
            parameters.Add("@limitSkip", skip);
            parameters.Add("@limitTake", take);
        }
        var items = _databaseContext.GetAllByQuery<FileEntity>(sqlExpression, parameters);
        return items;
    }

    public uint GetItemsCount(string? name = null, string? path = null, uint computerID = 0)
    {
        var parameters = new DynamicParameters();
        var conditions = new List<string> { "IsDeleted=0" };

        if (!string.IsNullOrEmpty(name))
        {
            conditions.Add("FileName=@FileName");
            parameters.Add("@FileName", name);
        }

        if (!string.IsNullOrEmpty(path))
        {
            conditions.Add("Path=@Path");
            parameters.Add("@Path", path);
        }

        if (computerID > 0)
        {
            conditions.Add("ComputerID=@ComputerID");
            parameters.Add("@ComputerID", computerID);
        }
        var sqlExpression = $"SELECT COUNT(*) FROM files WHERE {string.Join(" AND ", conditions)}";
        var itemsCount = _databaseContext.ExecuteByQuery(sqlExpression, parameters);

        return itemsCount;
    }

    private bool CreateItem(FileEntity item)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ComputerID", item.ComputerID);
        parameters.Add("@Path", item.Path);
        parameters.Add("@Name", item.FileName);

        const string sqlExpression = "INSERT INTO files (ComputerID, Path, FileName) " +
                                     "VALUES (@ComputerID, @Path, @Name)";
        _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForID);
        item.ID = id;
        return id > 0;
    }

    private bool UpdateItem(FileEntity item)
    {
        var sqlExpression = "UPDATE files SET " +
                            $"ComputerID = {item.ComputerID}, " +
                            $"Path = '{item.Path}', " +
                            $"FileName = '{item.FileName}', " +
                            $"IsDeleted = {(item.IsDeleted ? 1 : 0)} " +
                            $"WHERE ID = {item.ID}";
        var countOfChanges = _databaseContext.ExecuteByQuery(sqlExpression);
        return countOfChanges > 0;
    }
}

//public uint DeleteItem(uint id)
//{
//    if (id == 0) return 0;

//    var parameters = new DynamicParameters();
//    parameters.Add("@ID", id);
//    const string sqlExpressions = "UPDATE files SET IsDeleted = 1 WHERE ID = @ID";
//    var success = _databaseContext.ExecuteByQuery(sqlExpressions, parameters);
//    return success;
//}