using DB.Entities;

namespace DB.Repositories;

public class ComputerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public ComputerRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public dynamic CreateComputer(string name, int statusId, int employerId, DateTime date,
                                    string cpu, decimal price)
    {
        var sqlExpression = "INSERT INTO technick (Name, StatusID, EmployerID, DateCreated, Cpu, Price) " +
                            $"VALUES ('{name}', {statusId}, {employerId}, '{date.ToString("yyyy-MM-dd HH:mm:ss")}', '{cpu}', '{price}')";
        var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteExp(sqlExpression);
        var id = _databaseContext.ExecuteScalar(sqlExpressionForId);
        return id;
    }

    public dynamic ChangeComputer(int id, string name, int statusId, int employerId, DateTime date,
                                    string cpu, decimal price)
    {
        var sqlExpression = $"UPDATE technick SET Name = '{name}', " +
                            $"StatusID = {statusId}, " +
                            $"EmployerID = {employerId}, " +
                            $"DateCreated = '{date:yyyy-MM-dd HH:mm:ss}', " +
                            $"Cpu = '{cpu}', " +
                            $"Price = '{price}' " +
                            $"WHERE ID = {id}";
        var success = _databaseContext.ExecuteExp(sqlExpression);
        return success;
    }

    public Computer GetComputer(int id)
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





    //удалить
    

}