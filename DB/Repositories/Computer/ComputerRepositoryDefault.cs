using System.Globalization;

namespace DB.Repositories.Computer;
using Entities;

public class ComputerRepositoryDefault : IComputerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public ComputerRepositoryDefault()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void CreateComputer(Computer computer)
    {
        var sqlExpression = "INSERT INTO technick (Name, StatusID, EmployerID, DateCreated, Cpu, Price) " +
                            $"VALUES ('{computer.Name}', {computer.Status}, {computer.EmployerId}, '{computer.Date.ToString("yyyy-MM-dd HH:mm:ss")}', '{computer.Cpu}', '{computer.Price}')";
        var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        computer.Id = id;
    }

    public dynamic ChangeComputer(Computer computer)
    {
        var sqlExpression = $"UPDATE technick SET Name = '{computer.Name}', " +
                            $"StatusID = {computer.Status}, " +
                            $"EmployerID = {computer.EmployerId}, " +
                            $"DateCreated = '{computer.Date:yyyy-MM-dd HH:mm:ss}', " +
                            $"Cpu = '{computer.Cpu}', " +
                            $"Price = '{computer.Price.ToString(CultureInfo.InvariantCulture)}' " +
                            $"WHERE ID = {computer.Id}";
        var success = _databaseContext.ExecuteExp(sqlExpression);
        return success;
    }

    public Computer? GetComputer(int id)
    {
        var sqlExpression = $"SELECT * FROM technick WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
        var computer = _databaseContext.GetComputer(sqlExpression);
        return computer;
    }

    public List<Computer> GetComputers()
    {
        var sqlExpression = $"SELECT * FROM technick WHERE IsDeleted = 0";
        var computers = _databaseContext.GetComputers(sqlExpression);
        return computers;
    }

    public dynamic DeleteComputer(int id)
    {
        var sqlExpression = $"UPDATE technick SET IsDeleted = 1 WHERE ID = {id}";
        var result = _databaseContext.ExecuteExp(sqlExpression);
        return result;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}