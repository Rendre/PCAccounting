﻿using Dapper;
using DB.Entities;
using TaskStatus = DB.Entities.TaskStatus;

namespace DB.Repositories.Task;

public class TaskDapperRepository : ITaskRepository
{
    private readonly MySQLDatabaseContext _context;

    public TaskDapperRepository()
    {
        _context = new MySQLDatabaseContext();
    }

    public Entities.Task? GetItem(ulong id)
    {
        if (id == 0) return null;

        const string sqlExp = "SELECT * FROM Tasks WHERE ID=@ID AND IsDeleted=0";
        var parameters = new DynamicParameters();
        parameters.Add("@ID", id);
        var task = _context.GetByQuery<Entities.Task>(sqlExp);
        return task;
    }

    public IEnumerable<Entities.Task>? GetItems(string? name = null, TaskType type = 0, DateTime? date = null,
        TaskStatus status = TaskStatus.None)
    {
        //if (name == null &&
        //    type == 0 &&
        //    date == null &&
        //    status == 0) return null;
        var parameters = new DynamicParameters();
        var conditions = new List<string>(5)
        {
            "IsDeleted=0"
        };

        if (!string.IsNullOrEmpty(name))
        {
            conditions.Add("Name=@Name");
            parameters.Add("@Name", name);
        }

        if (type != TaskType.None)
        {
            conditions.Add("Type=@Type");
            parameters.Add("@Type", (byte)type);
        }

        if (date != null)
        {
            conditions.Add("Date=@Date");
            parameters.Add("@Date", date);
        }

        if (status != TaskStatus.None)
        {
            conditions.Add("Status=@Status");
            parameters.Add("@Status", (byte)status);
        }

        var sqlExp = $"SELECT * FROM Tasks WHERE {string.Join(" AND ", conditions)}";
        var tasks = _context.GetAllByQuery<Entities.Task>(sqlExp, parameters);
        return tasks;
    }
}