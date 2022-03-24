using Dapper;
using DB.Entities;

namespace DB.Repositories.Files;

public class FileDapperRepository : IFileRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public FileDapperRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void CreateItem(FileEntity? file)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ComputerID", file.ComputerID);
        parameters.Add("@Path", file.Path);
        parameters.Add("@Name", file.Name);

        const string sqlExpression = "INSERT INTO files (ComputerID, Path, FileName) " +
                                     "VALUES (@ComputerID, @Path, @Name)";
        _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForID);
        file.ID = id;
    }

    public FileEntity? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);

        const string sqlExpression = "SELECT * FROM files WHERE ID = @ID AND IsDeleted = 0";
        var fileFromDb = _databaseContext.GetByQuery<FileEntity>(sqlExpression, parameters);

        return fileFromDb;
    }

    public List<FileEntity?> GetItems(uint computerID = 0, string? orderBy = null, bool desc = false, uint limitSkip = 0, uint limitTake = 0)
    {
        var parameters = new DynamicParameters();
        var conditions = new List<string>(2) { "IsDeleted=0" };

        if (computerID > 0)
        {
            conditions.Add("ComputerID=@ComputerID");
            parameters.Add("@ComputerID", computerID);
        }
        var sqlExpression = $"SELECT * FROM files WHERE {string.Join(" AND ", conditions)}";
        if (!string.IsNullOrEmpty(orderBy) )
        {
            sqlExpression += $" ORDER BY {orderBy} {(desc ? "DESC" : "ASC")}";
        }

        if (limitTake > 0)
        {
            sqlExpression += " LIMIT @limitSkip, @limitTake";
            parameters.Add("@limitSkip", limitSkip);
            parameters.Add("@limitTake", limitTake);
        }
        List<FileEntity?> fileList = _databaseContext.GetAllByQuery<FileEntity>(sqlExpression, parameters);
        return fileList;
    }

    public uint DeleteItem(uint id)
    {
        if (id == 0) return 0;

        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        const string sqlExpressions = "UPDATE files SET IsDeleted = 1 WHERE ID = @ID";
        var success = _databaseContext.ExecuteByQuery(sqlExpressions, parameters);
        return success;
    }
}