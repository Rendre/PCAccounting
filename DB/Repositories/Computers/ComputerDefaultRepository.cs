using System.Globalization;
using DB.Entities;

namespace DB.Repositories.Computers;

public class ComputerDefaultRepository : IComputerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public ComputerDefaultRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void CreateItem(Computer computer)
    {
        var sqlExpression = "INSERT INTO technick (Name, StatusID, EmployerID, DateCreated, Cpu, Price) " +
                            $"VALUES ('{computer.Name}', {computer.StatusID}, {computer.EmployerId}, '{computer.DateCreated.ToString("yyyy-MM-dd HH:mm:ss")}', '{computer.Cpu}', '{computer.Price}')";
        const string sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        computer.ID = id;
    }

    public bool ChangeItem(Computer computer)
    {
        var sqlExpression = $"UPDATE technick SET Name = '{computer.Name}', " +
                            $"StatusID = {computer.StatusID}, " +
                            $"EmployerID = {computer.EmployerId}, " +
                            $"DateCreated = '{computer.DateCreated:yyyy-MM-dd HH:mm:ss}', " +
                            $"Cpu = '{computer.Cpu}', " +
                            $"Price = '{computer.Price.ToString(CultureInfo.InvariantCulture)}' " +
                            $"WHERE ID = {computer.ID}";
        var success = _databaseContext.ExecuteExp(sqlExpression);
        return success > 0;
    }

    public Computer? GetItem(uint id)
    {
        var sqlExpression = $"SELECT * FROM technick WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
        var computer = _databaseContext.GetComputer(sqlExpression);
        return computer;
    }

    public bool DeleteItem(uint id)
    {
        var sqlExpression = $"UPDATE technick SET IsDeleted = 1 WHERE ID = {id}";
        var result = _databaseContext.ExecuteExp(sqlExpression);
        return result > 0;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }

    public List<Computer> GetFilterItems(string? name = null, uint statusId = 0, uint employerId = 0, DateTime? date = null, string? cpu = null, decimal price = 0)
    {
        throw new NotImplementedException();
    }
}