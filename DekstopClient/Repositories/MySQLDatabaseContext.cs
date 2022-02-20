using DekstopClient.Entities;
using MySql.Data.MySqlClient;

namespace DekstopClient.Repositories;

public class MySQLDatabaseContext : IDisposable
{
    private const string ConnectionString = "server=127.0.0.1; database=retraincorp; charset=utf8; user id=root; password=root; pooling=false;";
    private readonly MySqlConnection _connection;

    public MySQLDatabaseContext()
    {
        _connection = new MySqlConnection(ConnectionString);
        _connection.Open();
    }

    public User? GetUser(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var user = new User()
        {
            //маппинг
            Id = reader.GetInt32(0), Login = reader.GetString(1),
            Password = reader.GetString(2), EmployerId = reader.GetInt32(3)
        };
        reader.Close();
        return user;
    }

    public Employer? GetEmployer(string sqlExpression)
    {
        var command = new MySqlCommand(sqlExpression, _connection);
        var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var employer = new Employer()
        {
            Id = reader.GetInt32(0), Name = reader.GetString(1),
            Position = reader.GetString(2), Tel = reader.GetString(3)
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
                Name = reader.GetString(1),
                Position = reader.GetString(2),
                Tel = reader.GetString(3)
            };
            employers.Add(employer);
        }
        reader.Close();
        return employers;
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}