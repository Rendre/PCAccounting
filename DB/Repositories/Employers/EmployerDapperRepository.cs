using System.Text;
using Dapper;
using DB.Entities;

namespace DB.Repositories.Employers;

public class EmployerDapperRepository : IEmployerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public EmployerDapperRepository(MySQLDatabaseContext mySQLDatabaseContext)
    {
        _databaseContext = mySQLDatabaseContext;
    }

    public bool SaveItem(Employer? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public Employer? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameters = new DynamicParameters();
        var conditions = new List<string>(2)
        {
            "IsDeleted = 0",
            "ID = @ID"
        };

        parameters.Add("@ID", id);

        var sqlExpression = $"SELECT * FROM employers WHERE {string.Join(" AND ", conditions)}";
        var item = _databaseContext.GetByQuery<Employer>(sqlExpression, parameters);
        return item;
    }

    public List<Employer> GetItems(string? name = null, string? position = null, string? tel = null,
        uint skip = 0, uint take = 0)
    {
        var sqlExpression = new StringBuilder("SELECT * FROM employers WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, parameters, name, position, tel, skip, take);
        var items = _databaseContext.GetAllByQuery<Employer>(sqlExpressionForQuery, parameters);
        return items;
    }

    public uint GetItemsCount(string? name = null, string? position = null, string? tel = null)
    {
        var sqlExpression = new StringBuilder("SELECT COUNT(*) FROM employers WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, parameters, name, position, tel);
        var itemsCount = _databaseContext.ExecuteByQuery(sqlExpressionForQuery, parameters);
        return itemsCount;
    }

    private static string GetParamForExpression(StringBuilder sqlExpression, DynamicParameters parameters, string? name, string? position, string? tel,
                                                    uint skip = 0, uint take = 0)
    {
        if (name != null)
        {
            sqlExpression.Append(" AND Name = @Name");
            parameters.Add("@Name", name);
        }

        if (position != null)
        {
            sqlExpression.Append(" AND Position = @Position");
            parameters.Add("@StatusID", position);
        }

        if (tel != null)
        {
            sqlExpression.Append(" AND Tel = @Tel");
            parameters.Add("@Tel", tel);
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

    private bool CreateItem(Employer employer)
    {
        var sqlExpression = "INSERT INTO employers (Name, Position, Tel)" +
                            $"VALUES ('{employer.Name}', '{employer.Position}', '{employer.Tel}')";
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForID);
        employer.ID = id;
        return id > 0;
    }

    private bool UpdateItem(Employer employer)
    {
        var sqlExpression = $"UPDATE employers SET Name = '{employer.Name}', Position = '{employer.Position}', Tel = '{employer.Tel}', IsDeleted = {employer.IsDeleted}" +
                            $" WHERE ID = {employer.ID}";
        var rowsChanged = _databaseContext.ExecuteByQuery(sqlExpression);
        return rowsChanged > 0;
    }
}