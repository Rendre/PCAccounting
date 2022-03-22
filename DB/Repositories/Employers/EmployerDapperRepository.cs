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

    public void CreateItem(Employer employer)
    {
        var sqlExpression = "INSERT INTO employers (Name, Position, Tel)" +
                            $"VALUES ('{employer.Name}', '{employer.Position}', '{employer.Tel}')";
        const string sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
        employer.ID = id;
    }

    public bool ChangeItem(Employer employer)
    {
        var sqlExpression = $"UPDATE employers SET Name = '{employer.Name}', Position = '{employer.Position}', Tel = '{employer.Tel}'" +
                            $" WHERE ID = {employer.ID}";
        var rowsChanged = _databaseContext.ExecuteByQuery(sqlExpression);
        return rowsChanged > 0;
    }

    public Employer? GetItem(uint id)
    {
        if (id == 0)
        {
            return null;
        }

        var parameters = new DynamicParameters();
        var conditions = new List<string>(2)
        {
            "IsDeleted = 0",
            "ID = @ID"
        };


        parameters.Add("@ID", id);


        var sqlExpression = $"SELECT * FROM employers WHERE {string.Join(" AND ", conditions)}";
        var employer = _databaseContext.GetByQuery<Employer>(sqlExpression, parameters);
        return employer;
    }

    //запрос с фильтром
    public List<Employer> GetItems(string? name = null, string? position = null, string? tel = null)
    {
        var parameters = new DynamicParameters();
        var conditions = new List<string>(4) { "IsDeleted = 0" };

        if (!string.IsNullOrEmpty(name))
        {
            // часть sql запроса, на место @Name подставится
            // значение из parameters - лежащее там под соотв. ключем
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
        var employers = _databaseContext.GetAllByQuery<Employer>(sqlExpression, parameters);
        return employers;
    }

    public uint DeleteItem(uint id)
    {
        if (id == 0)
        {
            return 0;
        }

        var parameters = new DynamicParameters();
        var conditions = new List<string>(1)
        {
            "ID = @ID"
        };

        parameters.Add("@ID", id);


        var sqlExpression = $"UPDATE employers SET IsDeleted = 1 WHERE {string.Join(" ", conditions)}";
        return _databaseContext.ExecuteByQuery(sqlExpression, parameters);
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }

    //public List<Employer> GetItems()
    //{
    //    throw new NotImplementedException();
    //}
}

//public Employer GetItem(int id)
//{
//    var sqlExpression = $"SELECT * FROM employers WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
//    var employer = _databaseContext.GetByQuery<Employer>(sqlExpression);
//    return employer;
//}

//public int DeleteItem(int id)
//{
//    var sqlExpression = $"UPDATE employers SET IsDeleted = 1 WHERE ID = {id}";
//    return _databaseContext.ExecuteByQuery(sqlExpression);
//}

//public List<Employer> GetItems()
//{
//    var sqlExpression = $"SELECT * FROM employers WHERE IsDeleted = 0";
//    //var employers = _databaseContext.GetEmployers(sqlExpression);
//    var employers = _databaseContext.GetAllByQuery<Employer>(sqlExpression);
//    return employers;
//}

//public int CreateEmployer(string? name, string? position, string? tel)
//{
//    var conditions = new List<string>(3);
//    var parameters = new DynamicParameters();

//    if (string.IsNullOrEmpty(name))
//    {
//        conditions.Add("@Name");
//        parameters.Add("@Name", name);
//    }

//    if (string.IsNullOrEmpty(position))
//    {
//        conditions.Add("@Position");
//        parameters.Add("@Position", position);
//    }

//    if (string.IsNullOrEmpty(tel))
//    {
//        conditions.Add("@Tel");
//        parameters.Add("@Tel", tel);
//    }

//    var sqlExpressions = $"INSERT INTO employers (Name, Position, Tel) VALUES ({string.Join(" ,", conditions)})";
//    _databaseContext.ExecuteByQuery(sqlExpressions, parameters);
//    var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
//    var resultId = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);

//    return resultId;
//}