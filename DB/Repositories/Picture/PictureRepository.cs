using Dapper;
namespace DB.Repositories.Picture;
using Entities;

public class PictureRepository : IPictureRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public PictureRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void SaveItem(Picture picture)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@CompID", picture.ComputerId);
        parameters.Add("@Path", picture.Path);
        parameters.Add("@Name", picture.Name);

        var sqlExpression = "INSERT INTO files (CompID, Path, FileName) " +
                            "VALUES (@CompID, @Path, @Name)";
        _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
        picture.Id = id;
    }

    public Picture? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);

        var sqlExpression = "SELECT * FROM files WHERE ID = @ID AND IsDeleted = 0";
        var pictureFromDb = _databaseContext.GetByQuery<Picture>(sqlExpression, parameters);

        return pictureFromDb;
    }

    public List<Picture> GetItems(uint computerId = 0, string? orderBy = null, bool desc = false, int limitSkip = 0, int limitTake = 0)
    {
        List<Picture> picturesList;
        var parameters = new DynamicParameters();
        var conditions = new List<string>(2) { "IsDeleted=0" };

        if (computerId > 0)
        {
            conditions.Add("CompID=@CompID");
            parameters.Add("@CompID", computerId);
        }
        //SELECT * FROM mytable ORDER BY column1 ASC, column2 DESC, column3 ASC
        string? sqlExpression;
        // переписать - на использование ордер бай + деск и лимит + скип + тейк
        if (!string.IsNullOrEmpty(orderBy))
        {
            parameters.Add("@limitSkip", limitSkip);
            parameters.Add("@limitTake", limitTake);
            // убывание
            if (desc)
            {
                sqlExpression = $"SELECT * FROM files WHERE {string.Join(" AND ", conditions)} " +
                                "ORDER BY ID DESC LIMIT @limitSkip, @limitTake";
                picturesList = _databaseContext.GetAllByQuery<Picture>(sqlExpression, parameters);
                return picturesList;
            }
            // возрастание
            sqlExpression = $"SELECT * FROM files WHERE {string.Join(" AND ", conditions)} " +
                            "ORDER BY ID ASC LIMIT @limitSkip, @limitTake";
            picturesList = _databaseContext.GetAllByQuery<Picture>(sqlExpression, parameters);
            return picturesList;
        }
        sqlExpression = $"SELECT * FROM files WHERE {string.Join(" AND ", conditions)}";
        picturesList = _databaseContext.GetAllByQuery<Picture>(sqlExpression, parameters);
        return picturesList;
    }

    public uint DeleteItem(uint id)
    {
        if (id == 0) return 0;

        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        var sqlExpressions = "UPDATE files SET IsDeleted = 1 WHERE ID = @ID";
        var success = _databaseContext.ExecuteByQuery(sqlExpressions, parameters);
        return success;
    }

}