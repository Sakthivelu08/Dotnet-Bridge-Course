using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.AdoNet
{
    public class AdoNetUnitOfWork : IUnitOfWork
    {
        public IRepository<Student> Students { get; }
        public IRepository<Teacher> Teachers { get; }
        public IUserRepository Users { get; }

        public AdoNetUnitOfWork(
            IRepository<Student> students,
            IRepository<Teacher> teachers,
            IUserRepository users)
        {
            Students = students;
            Teachers = teachers;
            Users = users;
        }

        public void Save()
        {
        }

        public void Dispose()
        {
        }
    }
}
