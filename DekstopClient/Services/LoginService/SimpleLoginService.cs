﻿using DB.Entities;
using DB.Repositories.User;

namespace DekstopClient.Services.LoginService;

public class SimpleLoginService : ILoginService
{
    public User? Login(string? login)
    {
        var userRepository = new UserDefaultRepository();
        using (userRepository)
        {
            return userRepository.GetItem(login);
        }
    }
}
