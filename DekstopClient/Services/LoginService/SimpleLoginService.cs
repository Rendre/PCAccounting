using DekstopClient.Entities;
using DekstopClient.Repositories;

namespace DekstopClient.Services.LoginService;

public class SimpleLoginService : ILoginService
{
    public User? Login(string login)
    {
        var userRepository = new UserRepository();
        using (userRepository)
        {
            return userRepository.GetItem(login);
        }

    }
}
