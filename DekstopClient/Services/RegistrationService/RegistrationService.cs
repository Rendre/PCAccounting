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
            var userFromDb = _userRepository.GetItem(user.Login);
            if (userFromDb != null) return false;

            _userRepository.CreateItem(user);
            return true;
        }
    }
}