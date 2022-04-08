using System.Globalization;
using System.Text;
using Dapper;
using DB.Entities;

namespace DB.Repositories.Computers;

public class ComputerDapperRepository : IComputerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public ComputerDapperRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public bool SaveItem(Computer? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public Computer? GetItem(uint id)
    {
        if (id == 0) return null;

        var parameters = new DynamicParameters();
        var conditions = new List<string>(2)
        {
            "IsDeleted = 0",
            "ID = @ID"
        };
        parameters.Add("@ID", id);
        var sqlExpression = $"SELECT * FROM computers WHERE {string.Join(" AND ", conditions)}";
        var computer = _databaseContext.GetByQuery<Computer>(sqlExpression, parameters);
        return computer;
    }

    public List<Computer> GetItems(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0, uint skip = 0, uint take = 0)
    {
        var sqlExpression = new StringBuilder("SELECT * FROM computers WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, name, statusID, employerID, 
            date, cpu, price, parameters, skip, take);
        var computers = _databaseContext.GetAllByQuery<Computer>(sqlExpressionForQuery, parameters);
        return computers;
    }

    public int GetItemsCount(string? name = null, uint statusID = 0, uint employerID = 0,
        DateTime? date = null, string? cpu = null, decimal price = 0)
    {
        var sqlExpression = new StringBuilder("SELECT * FROM computers WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, name, statusID, employerID, date, cpu, price, parameters);
        var computers = _databaseContext.GetAllByQuery<Computer>(sqlExpressionForQuery, parameters);
        return computers.Count;
    }

    private static string GetParamForExpression(StringBuilder sqlExpression, string? name, uint statusID, uint employerID, DateTime? date,
        string? cpu, decimal price, DynamicParameters parameters, uint skip = 0, uint take = 0)
    {
        if (name != null)
        {
            sqlExpression.Append(" AND Name = @Name");
            parameters.Add("@Name", name);
        }

        if (statusID != 0)
        {
            sqlExpression.Append(" AND StatusID = @StatusID");
            parameters.Add("@StatusID", statusID);
        }

        if (employerID != 0)
        {
            sqlExpression.Append(" AND EmployerID = @EmployerID");
            parameters.Add("@EmployerID", employerID);
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

        if (take > 0)
        {
            sqlExpression.Append(" LIMIT take = @take");
            parameters.Add("@take", take);

            sqlExpression.Append(" OFFSET skip = @skip");
            parameters.Add("@skip", skip);
        }

        return sqlExpression.ToString();
    }

  private bool CreateItem(Computer computer)
    {
        var sqlExpression = "INSERT INTO computers (Name, StatusID, EmployerID, DateCreated, Cpu, Price) " +
                            $"VALUES ('{computer.Name}', {computer.StatusID}, {computer.EmployerID}, '{computer.DateCreated.ToString("yyyy-MM-dd HH:mm:ss")}', '{computer.Cpu}', '{computer.Price}')";
        const string sqlExpressionForID = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForID);
        computer.ID = id;
        return id > 0;
    }

    private bool UpdateItem(Computer computer)
    {
        var sqlExpression = $"UPDATE computers SET Name = '{computer.Name}', " +
                            $"StatusID = {computer.StatusID}, " +
                            $"EmployerID = {computer.EmployerID}, " +
                            $"DateCreated = '{computer.DateCreated:yyyy-MM-dd HH:mm:ss}', " +
                            $"Cpu = '{computer.Cpu}', " +
                            $"IsDeleted = {computer.IsDeleted}" +
                            $"Price = '{computer.Price.ToString(CultureInfo.InvariantCulture)}' " +
                            $"WHERE ID = {computer.ID}";
        var countOfChanges = _databaseContext.ExecuteByQuery(sqlExpression);
        return countOfChanges > 0;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}