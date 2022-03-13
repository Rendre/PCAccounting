﻿namespace DB.Repositories.Computer;
using System.Globalization;
using System.Text;
using Dapper;
using Entities;



public class ComputerDapperRepository : IComputerRepository
{
    private readonly MySQLDatabaseContext _databaseContext;

    public ComputerDapperRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    public void CreateItem(Computer computer)
    {
        var sqlExpression = "INSERT INTO technick (Name, StatusID, EmployerID, DateCreated, Cpu, Price) " +
                            $"VALUES ('{computer.Name}', {computer.StatusID}, {computer.EmployerId}, '{computer.DateCreated.ToString("yyyy-MM-dd HH:mm:ss")}', '{computer.Cpu}', '{computer.Price}')";
        const string sqlExpressionForId = "SELECT LAST_INSERT_ID()";
        _databaseContext.ExecuteByQuery(sqlExpression);
        var id = _databaseContext.ExecuteScalarByQuery(sqlExpressionForId);
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
        var success = _databaseContext.ExecuteByQuery(sqlExpression);
        return success > 0;
    }

    //динамич запрос
    public List<Computer> GetFilterItems(string? name = null, uint statusId = 0, uint employerId = 0, DateTime? date = null,
        string? cpu = null, decimal price = 0)
    {
        var sqlExpression = new StringBuilder("SELECT * FROM technick WHERE IsDeleted = 0");
        var parameters = new DynamicParameters();
        var sqlExpressionForQuery = GetParamForExpression(sqlExpression, name, statusId, employerId, date, cpu, price, parameters);
        var computers = _databaseContext.GetAllByQuery<Computer>(sqlExpressionForQuery, parameters);
        return computers;
    }

    private static string GetParamForExpression(StringBuilder sqlExpression, string? name, uint statusId, uint employerId, DateTime? date,
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
        var sqlExpression = $"SELECT * FROM technick WHERE {string.Join(" AND ", conditions)}";
        var computer = _databaseContext.GetByQuery<Computer>(sqlExpression, parameters);
        return computer;
    }

    public bool DeleteItem(uint id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        const string sqlExpression = "UPDATE technick SET IsDeleted = 1 WHERE ID = @ID";
        var result = _databaseContext.ExecuteByQuery(sqlExpression, parameters);
        return result > 0;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}