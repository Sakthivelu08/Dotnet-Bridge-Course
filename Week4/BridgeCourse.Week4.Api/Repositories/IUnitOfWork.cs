using System;
using BridgeCourse.Week4.Api.Models;

namespace BridgeCourse.Week4.Api.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Student> Students { get; }
        IRepository<Teacher> Teachers { get; }
        IUserRepository Users { get; }
        void Save();
    }
}
