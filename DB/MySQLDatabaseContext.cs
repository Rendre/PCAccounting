using Dapper;
using DB.Entities;
using MySql.Data.MySqlClient;

namespace DB;

public class MySQLDatabaseContext : IDisposable
{
    private const string ConnectionString = "server=127.0.0.1; database=retraincorp; charset=utf8; user id=root; password=root; pooling=false;";
    private readonly MySqlConnection _connection;

    public MySQLDatabaseContext()
    {
        _connection = new MySqlConnection(ConnectionString);
        _connection.Open();
    }

    //[Obsolete]
    #region User
    public User? GetUser(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return null;
        }


        var user = new User()
        {
            //маппинг
            Id = reader.GetInt32(0),
            IsDeleted = reader.GetBoolean(1),
            Login = reader.GetString(2),
            Password = reader.GetString(3),
            EmployerId = reader.GetInt32(4),
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
            var user = new User()
            {
                Id = reader.GetInt32(0),
                IsDeleted = reader.GetBoolean(1),
                Login = reader.GetString(2),
                Password = reader.GetString(3),
                EmployerId = reader.GetInt32(4),
            };
            users.Add(user);
        }
        reader.Close();
        return users;
    }

    #endregion
    //[Obsolete]
    #region Computer
    public Computer? GetComputer(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var computer = new Computer()
        {
            Id = reader.GetInt32(0),
            IsDeleted = reader.GetBoolean(1),
            Name = reader.GetString(2),
            Status = reader.GetInt32(3),
            EmployerId = reader.GetInt32(4),
            Date = reader.GetDateTime(5),
            Cpu = reader.GetString(6),
            Price = reader.GetDecimal(7)
        };
        reader.Close();
        return computer;
    }
    //[Obsolete]
    public List<Computer> GetComputers(string sqlExpression)
    {
        var computers = new List<Computer>();
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var computer = new Computer()
            {
                Id = reader.GetInt32(0),
                IsDeleted = reader.GetBoolean(1),
                Name = reader.GetString(2),
                Status = reader.GetInt32(3),
                EmployerId = reader.GetInt32(4),
                Date = reader.GetDateTime(5),
                Cpu = reader.GetString(6),
                Price = reader.GetDecimal(7)
            };
            computers.Add(computer);
        }
        reader.Close();
        return computers;
    }


    #endregion

    #region Employer
    //[Obsolete]
    public Employer? GetEmployer(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var employer = new Employer()
        {
            Id = reader.GetInt32(0),
            IsDeleted = reader.GetBoolean(1),
            Name = reader.GetString(2),
            Position = reader.GetString(3),
            Tel = reader.GetString(4)
        };
        reader.Close();
        return employer;
    }

    public T GetByQuery<T>(string sqlExpression)
    {
        return _connection.QueryFirstOrDefault<T>(sqlExpression);
    }

    //[Obsolete]
    public List<Employer> GetEmployers(string sqlExpression)
    {
        var employers = new List<Employer>();
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var employer = new Employer()
            {
                Id = reader.GetInt32(0),
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
    
    public List<T> GetAllByQuery<T>(string sqlExpression)
    {
        return _connection.Query<T>(sqlExpression).ToList();
    }

    #endregion
    //[Obsolete]
    public int ExecuteExp(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        return command.ExecuteNonQuery();
    }

    public int ExecuteByQuery(string sqlExpression)
    {
        return _connection.Execute(sqlExpression);
    }

    //[Obsolete]
    public int ExecuteScalar(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int ExecuteScalarByQuery(string sqlExpression)
    {
        return _connection.ExecuteScalar<int>(sqlExpression);
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}