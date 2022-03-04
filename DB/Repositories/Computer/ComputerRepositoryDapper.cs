namespace DB.Repositories.Computer;
using System.Globalization;
using System.Text;
using Dapper;
using Entities;



public class ComputerRepositoryDapper : IComputerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public ComputerRepositoryDapper()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void CreateComputer(Computer computer)
    {
        var sqlExpression = "INSERT INTO technick (Name, StatusID, EmployerID, DateCreated, Cpu, Price) " +
                            $"VALUES ('{computer.Name}', {computer.StatusID}, {computer.EmployerId}, '{computer.DateCreated.ToString("yyyy-MM-dd HH:mm:ss")}', '{computer.Cpu}', '{computer.Price}')";
        var sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
        computer.Id = id;
    }

    public dynamic ChangeComputer(Computer computer)
    {
        var sqlExpression = $"UPDATE technick SET Name = '{computer.Name}', " +
                            $"StatusID = {computer.StatusID}, " +
                            $"EmployerID = {computer.EmployerId}, " +
                            $"DateCreated = '{computer.DateCreated:yyyy-MM-dd HH:mm:ss}', " +
                            $"Cpu = '{computer.Cpu}', " +
                            $"Price = '{computer.Price.ToString(CultureInfo.InvariantCulture)}' " +
                            $"WHERE ID = {computer.Id}";
        var success = _databaseContext.ExecuteByQuery(sqlExpression);
        return success;
    }

    //динамич запрос
    public List<Computer> GetFilterComputers(string? name = null, uint statusId = 0, uint employerId = 0, DateTime? date = null,
        string? cpu = null, decimal price = 0)
    {
        StringBuilder sqlExpression = new StringBuilder("SELECT * FROM technick WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, name, statusId, employerId, date, cpu, price, parameters);
        List<Computer> computers = _databaseContext.GetAllByQuery<Computer>(sqlExpressionForQuery, parameters);
        return computers;
    }

    private string GetParamForExpression(StringBuilder sqlExpression, string? name, uint statusId, uint employerId, DateTime? date,
        string? cpu, decimal price, DynamicParameters parameters)
    {
        if (name != null)
        {
            sqlExpression.Append(" AND Name = @Name");
            parameters.Add("@Name", name);
        }

        if (statusId != 0)
        {
            sqlExpression.Append(" AND StatusId = @StatusId");
            parameters.Add("@StatusId", statusId);
        }

        if (employerId != 0)
        {
            sqlExpression.Append(" AND EmployerId = @EmployerId");
            parameters.Add("@EmployerId", employerId);
        }

        if (date != null)
        {
            sqlExpression.Append(" AND DateCreated = @DateCreated");
            parameters.Add("@DateCreated", date);
        }

        if (cpu != null)
        {
            sqlExpression.Append(" AND Cpu = @Cpu");
            parameters.Add("@Cpu", cpu);

        }

        if (price != 0)
        {
            sqlExpression.Append(" AND Price = @Price");
            parameters.Add("@Price", price);
        }

        return sqlExpression.ToString();
    }

    public Computer GetComputer(uint id)
    {
        if (id == 0) return null;

        var parameters = new DynamicParameters();
        var conditions = new List<string>(2) { "IsDeleted = 0" };

        conditions.Add("ID = @ID");
        parameters.Add("@ID", id);


        var sqlExpression = $"SELECT * FROM employers WHERE {string.Join(" AND ", conditions)}";
        var computer = _databaseContext.GetByQuery<Computer>(sqlExpression, parameters);
        return computer;
    }

    [Obsolete]
    public List<Computer> GetComputers()
    {
        var sqlExpression = "SELECT * FROM technick WHERE IsDeleted = 0";
        var computers = _databaseContext.GetAllByQuery<Computer>(sqlExpression);
        return computers;
    }

    public dynamic DeleteComputer(uint id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);

        var sqlExpression = "UPDATE technick SET IsDeleted = 1 WHERE ID = @ID";
        var result = _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        return result;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}

//public Computer GetComputer(int id)
//{
//    var sqlExpression = $"SELECT * FROM technick WHERE ID = {id} AND IsDeleted = 0 LIMIT 1";
//    var computer = _databaseContext.GetByQuery<Computer>(sqlExpression);
//    return computer;
//}

//public dynamic DeleteComputer(uint id)
//{
//    var sqlExpression = $"UPDATE technick SET IsDeleted = 1 WHERE ID = {id}";
//    var result = _databaseContext.ExecuteByQuery(sqlExpression);
//    return result;
//}

//public List<Computer> GetFilterComputers(string? name = null, int statusId = 0, int employerId = 0, DateTime date = default(DateTime),
//                                            string? cpu = null, decimal price = 0)
//{
//    StringBuilder sqlExpression = new StringBuilder("SELECT * FROM technick WHERE IsDeleted = 0");
//    var sqlExpressionForQuery = GetParamForExpression(sqlExpression, name, statusId, employerId, date, cpu, price);
//    List<Computer> computers = _databaseContext.GetAllByQuery<Computer>(sqlExpressionForQuery);
//    return computers;
//}

//private string GetParamForExpression(StringBuilder sqlExpression, string? name, int statusId, int employerId, DateTime date,
//    string? cpu, decimal price)
//{
//    if (name != null) sqlExpression.Append($" AND Name = '{name}'");
//    if (statusId != 0) sqlExpression.Append($" AND StatusId = {statusId}");
//    if (employerId != 0) sqlExpression.Append($" AND EmployerId = {employerId}");
//    if (date != default(DateTime)) sqlExpression.Append($" AND DateCreated = {date:yyyy-MM-dd HH:mm:ss}");
//    if (cpu != null) sqlExpression.Append($" AND Cpu = '{cpu}'");
//    if (price != 0) sqlExpression.Append($" AND Price = {price}");

//    return sqlExpression.ToString();
//}