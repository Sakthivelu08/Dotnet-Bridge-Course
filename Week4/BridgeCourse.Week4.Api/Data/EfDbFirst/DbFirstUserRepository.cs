using System.Linq;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;
using DbUser = BridgeCourse.Week4.Api.Data.EfDbFirst.Models.User;

namespace BridgeCourse.Week4.Api.Data.EfDbFirst
{
    public class DbFirstUserRepository : IUserRepository
    {
        private readonly EfDbFirstDbContext _context;

        public DbFirstUserRepository(EfDbFirstDbContext context)
        {
            _context = context;
        }

        public User? GetByUsername(string username)
        {
            var dbUser = _context.Users.SingleOrDefault(u => u.Username == username);
            return dbUser == null ? null : MapToDomain(dbUser);
        }

        public void Add(User user)
        {
            var dbUser = MapToDb(user);
            _context.Users.Add(dbUser);
        }

        private User MapToDomain(DbUser u)
        {
            return new User
            {
                Username = u.Username,
                PasswordHash = u.PasswordHash,
                PasswordSalt = u.PasswordSalt,
                Role = u.Role
            };
        }

        private DbUser MapToDb(User u)
        {
            return new DbUser
            {
                Username = u.Username,
                PasswordHash = u.PasswordHash,
                PasswordSalt = u.PasswordSalt,
                Role = u.Role
            };
        }
    }
}
