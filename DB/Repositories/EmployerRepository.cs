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


    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}