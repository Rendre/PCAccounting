using DekstopClient.Entities;

namespace DekstopClient.Services.LoginService
{
    internal interface ILoginService
    {
        public User? Login(string login, string password);
    }
}
