using System.Net;
using System.Net.Mail;
using DB;
using DB.Entities;
using DB.Repositories;
using SharedKernel.Utils;

namespace SharedKernel.Services.LoginService;

public class LoginService : ILoginService
{
    private const int TokenLifeTime = 100;
    private readonly ProjectProperties _projectProperties;
    private readonly IUnitOfWork _unitOfWork;

    public LoginService(IUnitOfWork unitOfWork)
    {
        _projectProperties = Util.GetProjectProperties()!;
        _unitOfWork = unitOfWork;
    }

    public (bool isValid, Session? session) IsSessionValid(string? token)
    {
        if (string.IsNullOrEmpty(token)) return (false, null);

        var session = _unitOfWork.SessionRepository.GetItems(token).FirstOrDefault();
        return session == null ? (false, null) : (session.Time.AddMinutes(TokenLifeTime) >= DateTime.UtcNow, session);
    }

    public Session? GetSession(string? token)
    {
        var (isValid, session) = IsSessionValid(token);
        switch (isValid)
        {
            case true:
                return session;
            case false when session != null:
                session.IsDeleted = true;
                _unitOfWork.SessionRepository.SaveItem(session);
                return null;
            default:
                return null;
        }
    }

    public Session? GetNewSession(string? login, string? password, string userIP)
    {
        if (string.IsNullOrEmpty(login) ||
            string.IsNullOrEmpty(password)) return null;

        password = Util.Encode(password);
        var user = _unitOfWork.UserRepository.GetItems(login, isActivated: EntityStatus.OnlyActive, take: 1).FirstOrDefault();

        if (user is not { Password: { } } ||
            !user.Password.Equals(password)) return null;

        var session = new Session
        {
            Token = Guid.NewGuid().ToString("D"),
            Time = DateTime.UtcNow,
            UserID = user.ID,
            UserIP = userIP
        };
        _unitOfWork.SessionRepository.SaveItem(session);
        return session;
    }

    public User? GetUser(string? token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var session = _unitOfWork.SessionRepository.GetItems(token, take: 1).FirstOrDefault();
        if (session == null) return null;

        var user = _unitOfWork.UserRepository.GetItem(session.UserID);
        if (user == null) return null;

        user.Password = null;
        return user;
    }

    public bool AccountActivation(string? login, string? userMail, string? confirmationСode)
    {
        if (string.IsNullOrEmpty(confirmationСode)) return false;

        var user = _unitOfWork.UserRepository.GetItems(login, userMail, true).FirstOrDefault();
        // если учетка новая (не подтвержденная)
        if (user != null &&
            !string.IsNullOrEmpty(user.ActivationCode) &&
            confirmationСode.Equals(user.ActivationCode))
        {
            user.IsActivated = true;
            user.ActivationCode = null;
            _unitOfWork.UserRepository.SaveItem(user);
            return true;
        }

        return false;
    }

    public bool CreateNewAccount(string? login, string? password, string? userMail)
    {
        if (string.IsNullOrEmpty(login) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(userMail) ||
            !Util.CheckEmail(userMail)) return false;

        var user = _unitOfWork.UserRepository.GetItems(login, userMail, true).FirstOrDefault();
        if (user != null) return false;

        var random = new Random();
        var activationCode = random.Next(100000, 1000000).ToString();
        user = new User
        {
            Login = login,
            Password = password,
            EmployerID = 0,
            Email = userMail,
            IsActivated = false,
            ActivationCode = activationCode
        };
        _unitOfWork.UserRepository.SaveItem(user);

        var from = new MailAddress(_projectProperties.MyEmail, _projectProperties.SendersName);
        var to = new MailAddress(userMail);
        var mailMessage = new MailMessage(from, to);
        mailMessage.Subject = "Регистрация";
        mailMessage.Body = $"<h2>Код подтверждения регистрации: {activationCode}</h2>";
        mailMessage.IsBodyHtml = true;
        var smtpClient = new SmtpClient(_projectProperties.AddressSMTP, _projectProperties.PortSMTP);
        smtpClient.Credentials =
            new NetworkCredential(_projectProperties.MyEmail, _projectProperties.PasswordForMyEmail);
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMessage);

        return true;
    }
}