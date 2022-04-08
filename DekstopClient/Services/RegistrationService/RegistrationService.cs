using DB.Entities;
using DB.Repositories.Users;

namespace DekstopClient.Services.RegistrationService;

public class RegistrationService : IRegistrationService
{
    private readonly IUserRepository _userRepository;

    public RegistrationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool Registration(User user)
    {
        using (_userRepository)
        {
            var users = _userRepository.GetItems(user.Login, take:1);
            var userFromDb = users.First();
            if (userFromDb != null) return false;

            _userRepository.SaveItem(user);
            return true;
        }
    }
}