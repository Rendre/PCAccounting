using DB.Entities;

namespace SharedKernel.Services.LoginService;

public interface ILoginService
{
    public (bool isValid, Session? session) IsSessionValid(string? token);

    public Session? GetSession(string? token);

    public Session? GetNewSession(string? login, string? password, string userIP);

    public User? GetUser(string? token);

    public bool AccountActivation(string? login, string? userMail, string? confirmationСode);

    public bool CreateNewAccount(string? login, string? password, string? userMail);
}