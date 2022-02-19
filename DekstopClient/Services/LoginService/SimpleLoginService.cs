using DekstopClient.Entities;
using MySql.Data.MySqlClient;

namespace DekstopClient.Services.LoginService;

public class SimpleLoginService : ILoginService
{
    public User? Login(string login, string password)
    {
        var str = string.Format(
            "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
            "retraincorp", "root", "root");
        using var connection = new MySqlConnection(str);
        var sqlExpression = $"SELECT * FROM Users WHERE Login = '{login}' AND Pass = '{password}' LIMIT 1";
        connection.Open();
        var command = new MySqlCommand(sqlExpression, connection);
        var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var user = new User { Id = reader.GetInt32(0), Login = reader.GetString(1), Password = reader.GetString(2), EmployerId = reader.GetInt32(3) };
            return user;
        }
        
        return null;
    }
}