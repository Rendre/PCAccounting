using Dapper;
using DB.Entities;
using MySqlConnector;

namespace DB;

public class MySQLDatabaseContext : IDisposable
{
    public const string ConnectionString = "server=127.0.0.1; database=retraincorp; charset=utf8; user id=root; password=root; pooling=false;";
    private readonly MySqlConnection _connection;

    public MySQLDatabaseContext()
    {
        _connection = new MySqlConnection(ConnectionString);
        _connection.Open();
    }

    public T? GetByQuery<T>(string sqlExp, DynamicParameters parameters)
    {
        return _connection.QueryFirstOrDefault<T>(sqlExp, parameters);
    }

    public List<T> GetAllByQuery<T>(string sqlExp, DynamicParameters parameters)
    {
        return (List<T>) _connection.Query<T>(sqlExp, parameters);
    }


    public uint ExecuteByQuery(string sqlExpression)
    {
        return (uint) _connection.Execute(sqlExpression);
    }

    public uint ExecuteByQuery(string sqlExpression, DynamicParameters dynamicParameters)
    {
        return (uint) _connection.Execute(sqlExpression, dynamicParameters);
    }

    public uint ExecuteScalarByQuery(string sqlExpression)
    {
        return _connection.ExecuteScalar<uint>(sqlExpression);
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    //[Obsolete]
    public Computer? GetComputer(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var computer = new Computer
        {
            ID = reader.GetUInt32(0),
            IsDeleted = reader.GetBoolean(1),
            Name = reader.GetString(2),
            StatusID = reader.GetUInt32(3),
            EmployerID = reader.GetUInt32(4),
            DateCreated = reader.GetDateTime(5),
            Cpu = reader.GetString(6),
            Price = reader.GetDecimal(7)
        };
        reader.Close();
        return computer;
    }

    //[Obsolete]
    public User? GetUser(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return null;
        }


        var user = new User
        {
            //маппинг
            ID = reader.GetUInt32(0),
            IsDeleted = reader.GetBoolean(1),
            Login = reader.GetString(2),
            Password = reader.GetString(3),
            EmployerID = reader.GetUInt32(4),
            IsActivated = reader.GetBoolean(5),
            ActivationCode = reader.GetString(6),
            Email = reader.GetString(7)


        };
        reader.Close();
        return user;
    }
    //[Obsolete]
    public List<User> GetUsers(string sqlExpression)
    {
        var users = new List<User>();
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var user = new User
            {
                ID = reader.GetUInt32(0),
                IsDeleted = reader.GetBoolean(1),
                Login = reader.GetString(2),
                Password = reader.GetString(3),
                EmployerID = reader.GetUInt32(4),
                IsActivated = reader.GetBoolean(5),
                Email = reader.GetString(7)
            };
            users.Add(user);
        }
        reader.Close();
        return users;
    }

    //[Obsolete]
    public uint ExecuteScalar(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        return Convert.ToUInt32(command.ExecuteScalar());
    }

    //[Obsolete]
    public uint ExecuteExp(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        return (uint)command.ExecuteNonQuery();
    }

    //[Obsolete]
    public List<T> GetAllByQuery<T>(string sqlExpression)
    {
        return _connection.Query<T>(sqlExpression).ToList();
    }

    //[Obsolete]
    public List<Employer> GetEmployers(string sqlExpression)
    {
        var employers = new List<Employer>();
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var employer = new Employer
            {
                ID = reader.GetUInt32(0),
                IsDeleted = reader.GetBoolean(1),
                Name = reader.GetString(2),
                Position = reader.GetString(3),
                Tel = reader.GetString(4)
            };
            employers.Add(employer);
        }
        reader.Close();
        return employers;
    }

    //[Obsolete]
    public T? GetByQuery<T>(string sqlExpression)
    {
        return _connection.QueryFirstOrDefault<T>(sqlExpression);
    }

    //[Obsolete]
    public Employer? GetEmployer(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var employer = new Employer
        {
            ID = reader.GetUInt32(0),
            IsDeleted = reader.GetBoolean(1),
            Name = reader.GetString(2),
            Position = reader.GetString(3),
            Tel = reader.GetString(4)
        };
        reader.Close();
        return employer;
    }

    //[Obsolete]
    public List<Computer> GetComputers(string sqlExpression)
    {
        var computers = new List<Computer>();
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var computer = new Computer
            {
                ID = reader.GetUInt32(0),
                IsDeleted = reader.GetBoolean(1),
                Name = reader.GetString(2),
                StatusID = reader.GetUInt32(3),
                EmployerID = reader.GetUInt32(4),
                DateCreated = reader.GetDateTime(5),
                Cpu = reader.GetString(6),
                Price = reader.GetDecimal(7)
            };
            computers.Add(computer);
        }
        reader.Close();
        return computers;
    }
}