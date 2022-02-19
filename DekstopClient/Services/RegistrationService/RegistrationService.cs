using MySql.Data.MySqlClient;

namespace DekstopClient.Services.RegistrationService;

public class RegistrationService : IRegistrationService
{
    public bool Registration(string login, string password)
    {
        var str = string.Format(
            "server={0}; database={1}; charset=utf8; user id={2}; password={3}; pooling=false;", "127.0.0.1",
            "retraincorp", "root", "root");
        using var connection = new MySqlConnection(str);
        var sqlExpression = $"SELECT * FROM Users WHERE Login = '{login}' LIMIT 1";
        connection.Open();
        var command = new MySqlCommand(sqlExpression, connection);
        var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return false;
        }
        else
        {
            reader.Close();
            sqlExpression = $"INSERT INTO users (Login, Pass) VALUES ('{login}', '{password}')";
            command = new MySqlCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
            return true;
        }
    }
}