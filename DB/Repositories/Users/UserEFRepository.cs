﻿using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories.Users;

public class UserEFRepository : IUserRepository
{
    private readonly ApplicationContextEF _db;
    public UserEFRepository()
    {
        var opt = new DbContextOptionsBuilder<ApplicationContextEF>();
        opt.UseMySql(MySQLDatabaseContext.ConnectionString, ApplicationContextEF.ServerVersion);
        _db = new ApplicationContextEF(opt.Options);
    }

    public bool SaveItem(User? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public User? GetItem(uint id)
    {
        if (id == 0) return null;

        var item = _db.Users.FirstOrDefault(p => p.ID == id && p.IsDeleted == false);
        return item;
    }

    public List<User> GetItems(string? login = null, string? email = null, uint employerID = 0,
        EntityStatus isActivated = EntityStatus.None, string? activationCode = null, uint skip = 0, uint take = 0)

    {
        var items = _db.Users.Where(p => p.IsDeleted == false);

        if (isActivated != EntityStatus.None)
        {
            var check = isActivated == EntityStatus.OnlyActive;
            items = _db.Users.Where(p => p.IsActivated == check);
        }

        if (!string.IsNullOrEmpty(login))
        {
            items = items.Where(p => p.Login != null && p.Login.Equals(login));
        }

        if (!string.IsNullOrEmpty(email))
        {
            items = items.Where(p => p.Email != null && p.Email.Equals(email));
        }

        if (employerID > 0)
        {
            items = items.Where(p => p.EmployerID == employerID);
        }

        if (!string.IsNullOrEmpty(activationCode))
        {
            items = items.Where(p => p.ActivationCode != null && p.ActivationCode.Equals(activationCode));
        }

        if (take > 0)
        {
            items = items.Skip((int)skip).Take((int)take);
        }

        return items.ToList();
    }

    public int GetItemsCount(string? login = null, string? email = null, uint employerID = 0, EntityStatus isActivated = EntityStatus.None,
        string? activationCode = null)
    {
        var items = _db.Users.Where(p => p.IsDeleted == false);

        if (isActivated != EntityStatus.None)
        {
            var check = isActivated == EntityStatus.OnlyActive;
            items = _db.Users.Where(p => p.IsActivated == check);
        }

        if (!string.IsNullOrEmpty(login))
        {
            items = items.Where(p => p.Login != null && p.Login.Equals(login));
        }

        if (!string.IsNullOrEmpty(email))
        {
            items = items.Where(p => p.Email != null && p.Email.Equals(email));
        }

        if (employerID > 0)
        {
            items = items.Where(p => p.EmployerID == employerID);
        }

        if (!string.IsNullOrEmpty(activationCode))
        {
            items = items.Where(p => p.ActivationCode != null && p.ActivationCode.Equals(activationCode));
        }

        return items.Count();
    }

    private bool CreateItem(User user)
    {
        _db.Users.Add(user);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }

    private bool UpdateItem(User user)
    {
        _db.Users.Update(user);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

//public User? GetItem(string? login)
//{
//    if (string.IsNullOrEmpty(login)) return null;

//    var user = _db.Users.FirstOrDefault(p => p.Login != null && p.Login.Equals(login) &&
//                                             p.IsDeleted == false);
//    return user;
//}

//public User? GetItemByEmail(string? email)
//{
//    var user = _db.Users.FirstOrDefault(p => p.Email != null && p.Email.Equals(email));
//    return user;
//}

//public List<User> GetItems()
//{
//    var users = _db.Users.Where(p => p.IsDeleted == false);
//    return users.ToList();
//}

//public bool DeleteItem(uint id)
//{
//    var user = _db.Users.FirstOrDefault(p => p.ID == id);
//    if (user == null) return false;

//    user.IsDeleted = true;
//    _db.Users.Update(user);
//    var rowsChanged = _db.SaveChanges();
//    return rowsChanged > 0;
//}
