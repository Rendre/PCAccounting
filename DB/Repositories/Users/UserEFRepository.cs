using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories.Users
{
    public class UserEFRepository : IUserRepository
    {
        private readonly ApplicationContextEF _db;
        public UserEFRepository()
        {
            var opt = new DbContextOptionsBuilder<ApplicationContextEF>();
            opt.UseMySql(MySQLDatabaseContext.ConnectionString, ApplicationContextEF.ServerVersion);
            _db = new ApplicationContextEF(opt.Options);
        }

        public bool CreateItem(User user)
        {
            _db.Users.Add(user);
            var stateCount = _db.SaveChanges();
            return stateCount > 0;
        }

        public bool UpdateItem(User user)
        {
            _db.Users.Update(user);
            var stateCount = _db.SaveChanges();
            return stateCount > 0;
        }

        public User? GetItem(uint id)
        {
            var user = _db.Users.FirstOrDefault(p => p.ID == id);
            return user;
        }

        public User? GetItem(string? login)
        {
            var user = _db.Users.FirstOrDefault(p => p.Login != null && p.Login.Equals(login));
            return user;
        }

        public User? GetItemByEmail(string? email)
        {
            var user = _db.Users.FirstOrDefault(p => p.Email != null && p.Email.Equals(email));
            return user;
        }

        public List<User> GetItems()
        {
            var users = _db.Users.Where(p => p.IsDeleted == false);
            return users.ToList();
        }

        public bool DeleteItem(uint id)
        {
            var user = _db.Users.FirstOrDefault(p => p.ID == id);
            if (user == null) return false;

            user.IsDeleted = true;
            _db.Users.Update(user);
            var rowsChanged = _db.SaveChanges();
            return rowsChanged > 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
