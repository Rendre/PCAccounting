using DB.Entities;

namespace DB.Repositories;

public class EmployerRepository : IDisposable
{
    private readonly MySQLDatabaseContext _databaseContext;

    public EmployerRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public Employer? GetItem(int id)
    {
        var sqlExpression = $"SELECT * FROM employers WHERE ID = {id} LIMIT 1";
        var employer = _databaseContext.GetEmployer(sqlExpression);
        return employer;
    }

    public List<Employer> GetItems()
    {
        var sqlExpression = $"SELECT * FROM employers";
        var employers = _databaseContext.GetEmployers(sqlExpression);
        return employers;
    }

    public int DeleteItem(int id)
    {
        var sqlExpression = $"DELETE FROM employers WHERE ID = {id}";
        return _databaseContext.ExecuteExp(sqlExpression);
    }

    public int CreateEmployer(string? name, string? position, string? tel)
    {
        var sqlExpression = $"INSERT INTO employers (Name, Position, Tel)" +
                            $"VALUES ('{name}', '{position}', '{tel}')";
        var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        return id;

    }

    public int СhangeEmployer(int id, string? name, string? position, string? tel)
    {
        var sqlExpression = $"UPDATE employers SET Name = '{name}', Position = '{position}', Tel = '{tel}' WHERE ID = {id}";
        var success = _databaseContext.ExecuteExp(sqlExpression);
        return success;

    }


    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }

    public object СhangeEmployer(string? name, string? position, string? tel)
    {
        throw new NotImplementedException();
    }
}