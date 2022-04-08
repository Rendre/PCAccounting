using DB.Entities;
using DB.Repositories.Users;

namespace DekstopClient.Services.LoginService;

public class SimpleLoginService : ILoginService
{
    public User Login(string? login)
    {
        var userRepository = new UserDapperRepository();
        using (userRepository)
        {
            var users = userRepository.GetItems(login, take: 1);
            return users.First();
        }
    }
}
