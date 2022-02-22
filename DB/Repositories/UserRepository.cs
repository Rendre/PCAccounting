using DB.Entities;

namespace DB.Repositories;

public class UserRepository : IDisposable
{
    private readonly MySQLDatabaseContext _databaseContext;

    public UserRepository()
    {
        _databaseContext = new MySQLDatabaseContext();
    }

    //возвр юзера из бд по айдишнику (метод)
    //здесь формируем запросы - и отправляем их в контекст
    public User? GetItem(int id)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE ID = {id} LIMIT 1";
            var user = _databaseContext.GetUser(sqlExpression);
            return user;
    }

    public User? GetItem(string login)
    {
        var sqlExpression = $"SELECT * FROM Users WHERE Login = '{login}' LIMIT 1";
        var user = _databaseContext.GetUser(sqlExpression);
        return user;
    }

    public void Dispose()
    {
        _databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
