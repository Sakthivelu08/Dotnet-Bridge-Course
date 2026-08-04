using System;
using BridgeCourse.Week2.Api.Models;

namespace BridgeCourse.Week2.Api.Repositories
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        public IRepository<Student> Students { get; }
        public IRepository<Teacher> Teachers { get; }

        public InMemoryUnitOfWork()
        {
            Students = new InMemoryRepository<Student>();
            Teachers = new InMemoryRepository<Teacher>();
        }

        public void Save()
        {
            Console.WriteLine("[Unit of Work] Save completed successfully (committed changes).");
        }

        public void Dispose()
        {
        }
    }
}
