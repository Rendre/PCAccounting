using DB.Entities;

namespace DB.Repositories.Employers;

public class EmployerDefaultRepository : IEmployerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public EmployerDefaultRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public Employer? GetItem(uint id)
    {
        var sqlExpression = $"SELECT * FROM employers WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
        var employer = _databaseContext.GetEmployer(sqlExpression);
        return employer;
    }

    public List<Employer> GetItems()
    {
        const string sqlExpression = "SELECT * FROM employers WHERE IsDeleted = 0";
        //var employers = _databaseContext.GetEmployers(sqlExpression);
        var employers = _databaseContext.GetEmployers(sqlExpression);
        return employers;
    }

    public List<Employer> GetItems(string? name = null, string? position = null, string? tel = null)
    {
        throw new NotImplementedException();
    }

    public uint DeleteItem(uint id)
    {
        var sqlExpression = $"UPDATE employers SET IsDeleted = 1 WHERE ID = {id}";
        return _databaseContext.ExecuteExp(sqlExpression);
    }

    public void CreateItem(Employer employer)
    {
        var sqlExpression = "INSERT INTO employers (Name, Position, Tel)" +
                            $"VALUES ('{employer.Name}', '{employer.Position}', '{employer.Tel}')";
        const string sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        employer.ID = id;
    }

    public bool ChangeItem(Employer employer)
    {
        var sqlExpression = $"UPDATE employers SET Name = '{employer.Name}', Position = '{employer.Position}', Tel = '{employer.Tel}'" +
                            $" WHERE ID = {employer.ID}";
        var success = _databaseContext.ExecuteExp(sqlExpression);
        return success > 0;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }

    //List<Employer> IEmployerRepository.GetItems()
    //{
    //    throw new NotImplementedException();
    //}
}