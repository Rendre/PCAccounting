using Dapper;
using DB.Entities;

namespace DB.Repositories.Employers;

public class EmployerDapperRepository : IEmployerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public EmployerDapperRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
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
        var parameters = new DynamicParameters();
        var conditions = new List<string>(4) { "IsDeleted = 0" };

        if (!string.IsNullOrEmpty(name))
        {
            conditions.Add("Name = @Name");
            parameters.Add("@Name", name);
        }

        if (!string.IsNullOrEmpty(position))
        {
            conditions.Add("Position = @Position");
            parameters.Add("@Position", position);
        }

        if (!string.IsNullOrEmpty(tel))
        {
            conditions.Add("Tel = @Tel");
            parameters.Add("@Tel", tel);
        }

        var sqlExpression = $"SELECT * FROM employers WHERE {string.Join(" AND ", conditions)}";
        var items = _databaseContext.GetAllByQuery<Employer>(sqlExpression, parameters);
        return items;
    }

    public uint GetItemsCount(string? name = null, string? position = null, string? tel = null)
    {
        var parameters = new DynamicParameters();
        var conditions = new List<string>(4) { "IsDeleted = 0" };

        if (!string.IsNullOrEmpty(name))
        {
            conditions.Add("Name = @Name");
            parameters.Add("@Name", name);
        }

        if (!string.IsNullOrEmpty(position))
        {
            conditions.Add("Position = @Position");
            parameters.Add("@Position", position);
        }

        if (!string.IsNullOrEmpty(tel))
        {
            conditions.Add("Tel = @Tel");
            parameters.Add("@Tel", tel);
        }

        var sqlExpression = $"SELECT COUNT(*) FROM employers WHERE {string.Join(" AND ", conditions)}";
        var itemsCount = _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        return itemsCount;
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

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }

}