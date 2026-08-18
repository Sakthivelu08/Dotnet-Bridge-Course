using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.EfDbFirst
{
    public class DbFirstUnitOfWork : IUnitOfWork
    {
        private readonly EfDbFirstDbContext _context;
        public IRepository<Student> Students { get; }
        public IRepository<Teacher> Teachers { get; }
        public IUserRepository Users { get; }

        public DbFirstUnitOfWork(
            EfDbFirstDbContext context,
            IRepository<Student> students,
            IRepository<Teacher> teachers,
            IUserRepository users)
        {
            _context = context;
            Students = students;
            Teachers = teachers;
            Users = users;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
