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

        var sqlExpression = "INSERT INTO files (CompID, Path) " +
                            "VALUES (@CompID, @Path)";
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

    public List<Picture> GetItems(uint computerId = 0)
    {
        var parameters = new DynamicParameters();
        var conditions = new List<string>(2) {"IsDeleted=0"};

        if (computerId > 0)
        {
            conditions.Add("CompID=@CompID");
            parameters.Add("@CompID", computerId);
        }

        var sqlExpression = $"SELECT * FROM files WHERE {string.Join(" AND ", conditions)}";
        var picturesList = _databaseContext.GetAllByQuery<Picture>(sqlExpression, parameters);
        return picturesList;
    }

    public uint DeleteItem(uint id)
    {
        if(id == 0) return 0;

        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        var sqlExpressions = "UPDATE files SET IsDeleted = 1 WHERE ID = @ID";
        var success = _databaseContext.ExecuteByQuery(sqlExpressions, parameters);
        return success;
    }

}