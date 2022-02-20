using DekstopClient.Entities;
using DekstopClient.Repositories;
using MySql.Data.MySqlClient;

namespace DekstopClient.Services.LoginService;

public class SimpleLoginService : ILoginService
{
    public User? Login(string login, string password)
    {
        var userRepository = new UserRepository();
        using (userRepository)
        {
            return userRepository.GetItem(login, password);
        }

    }
}
