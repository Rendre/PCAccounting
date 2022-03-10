using DB.Repositories.User;

namespace DekstopClient.Services.RegistrationService;

public class RegistrationService : IRegistrationService
{
    private readonly IUserRepository _userRepository;

    public RegistrationService(IUserRepository userRepository)
    { 
        _userRepository = userRepository;
    }

    public bool Registration(string? login, string? password)
    {
        using (_userRepository)
        {
            var user = _userRepository.GetItem(login);
            if (user != null)
            {
                return false;
            }

            _userRepository.CreateUser(login, password, 0);
            return true;
        }
    }
}