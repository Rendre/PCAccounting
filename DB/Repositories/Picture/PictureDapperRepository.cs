namespace DB.Repositories.Picture;
using Dapper;
using Entities;

public class PictureDapperRepository : IPictureRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public PictureDapperRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void SaveItem(Picture? picture)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@CompID", picture.ComputerId);
        parameters.Add("@Path", picture.Path);
        parameters.Add("@Name", picture.Name);

        const string sqlExpression = "INSERT INTO files (CompID, Path, FileName) " +
                                     "VALUES (@CompID, @Path, @Name)";
        _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        const string sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
        picture.ID = id;
    }

    public Picture? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);

        const string sqlExpression = "SELECT * FROM files WHERE ID = @ID AND IsDeleted = 0";
        var pictureFromDb = _databaseContext.GetByQuery<Picture>(sqlExpression, parameters);

        return pictureFromDb;
    }

    public List<Picture?> GetItems(uint computerId = 0, string? orderBy = null, bool desc = false, uint limitSkip = 0, uint limitTake = 0)
    {
        var parameters = new DynamicParameters();
        var conditions = new List<string>(2) { "IsDeleted=0" };

        if (computerId > 0)
        {
            conditions.Add("CompID=@CompID");
            parameters.Add("@CompID", computerId);
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
        List<Picture?> picturesList = _databaseContext.GetAllByQuery<Picture>(sqlExpression, parameters);
        return picturesList;
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