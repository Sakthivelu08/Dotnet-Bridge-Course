using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.EfCodeFirst
{
    public class EfCodeFirstUnitOfWork : IUnitOfWork
    {
        private readonly EfCodeFirstDbContext _context;
        public IRepository<Student> Students { get; }
        public IRepository<Teacher> Teachers { get; }
        public IUserRepository Users { get; }

        public EfCodeFirstUnitOfWork(
            EfCodeFirstDbContext context,
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
