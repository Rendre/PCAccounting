namespace DekstopClient.Services.RegistrationService;

public interface IRegistrationService
{
    public bool Registration(string login, string password);
}