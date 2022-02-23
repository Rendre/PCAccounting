using DB.Entities;
using MySql.Data.MySqlClient;

namespace DB.Repositories;

public class MySQLDatabaseContext : IDisposable
{
    private const string ConnectionString = "server=127.0.0.1; database=retraincorp; charset=utf8; user id=root; password=root; pooling=false;";
    private readonly MySqlConnection _connection;

    public MySQLDatabaseContext()
    {
        _connection = new MySqlConnection(ConnectionString);
        _connection.Open();
    }


    #region User
    public User? GetUser(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

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



    public int ExecuteExp(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        return command.ExecuteNonQuery();
    }

    public int ExecuteScalar(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        return Convert.ToInt32(command.ExecuteScalar());
    }



    #region Employer
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

    #endregion

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}