using System.Linq;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.EfCodeFirst
{
    public class EfUserRepository : IUserRepository
    {
        private readonly EfCodeFirstDbContext _context;

        public EfUserRepository(EfCodeFirstDbContext context)
        {
            _context = context;
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }
    }
}
