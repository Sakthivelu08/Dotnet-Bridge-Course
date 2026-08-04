using System;
using BridgeCourse.Week2.Api.Models;

namespace BridgeCourse.Week2.Api.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Student> Students { get; }
        IRepository<Teacher> Teachers { get; }
        void Save();
    }
}
