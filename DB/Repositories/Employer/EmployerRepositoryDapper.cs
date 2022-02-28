namespace DB.Repositories.Employer;
using Entities;

public class EmployerRepositoryDapper :  IEmployerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public EmployerRepositoryDapper()
    {
        _databaseContext = new MySQLDatabaseContext();
    }
   
    public Employer GetItem(int id)
    {
        var sqlExpression = $"SELECT * FROM employers WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
        var employer = _databaseContext.GetByQuery<Employer>(sqlExpression);
        return employer;
    }
  
    public List<Employer> GetItems()
    {
        var sqlExpression = $"SELECT * FROM employers WHERE IsDeleted = 0";
        //var employers = _databaseContext.GetEmployers(sqlExpression);
        var employers = _databaseContext.GetAllByQuery<Employer>(sqlExpression);
        return employers;
    }

    public int DeleteItem(int id)
    {
        var sqlExpression = $"UPDATE employers SET IsDeleted = 1 WHERE ID = {id}";
        return _databaseContext.ExecuteByQuery(sqlExpression);
    }
    
    public int CreateEmployer(string? name, string? position, string? tel)
    {
        var sqlExpression = $"INSERT INTO employers (Name, Position, Tel)" +
                            $"VALUES ('{name}', '{position}', '{tel}')";
        var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
        return id;
    }
   
    public int СhangeEmployer(int id, string? name, string? position, string? tel)
    {
        var sqlExpression = $"UPDATE employers SET Name = '{name}', Position = '{position}', Tel = '{tel}'" +
                            $" WHERE ID = {id}";
        var success = _databaseContext.ExecuteByQuery(sqlExpression);
        return success;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}