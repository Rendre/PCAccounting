using DB.Entities;

namespace DekstopClient.Services.RegistrationService;

public interface IRegistrationService
{
    public bool Registration(User user);
}