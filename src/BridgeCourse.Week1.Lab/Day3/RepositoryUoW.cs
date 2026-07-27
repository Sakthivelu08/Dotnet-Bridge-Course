using System;
using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week1.Lab.Day2;

namespace BridgeCourse.Week1.Lab.Day3
{
    #region Entities

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    #endregion

    #region Generic Repository Interface

    /// <summary>
    /// Generic repository interface outlining basic CRUD operations.
    /// This forms the abstract seam that future ORMs/DB drivers (like EF Core) will plug into.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }

    #endregion

    #region Concrete Repositories (In-Memory)

    /// <summary>
    /// In-memory repository for Student records.
    /// </summary>
    public class StudentRepository : IRepository<Student>
    {
        // Static in-memory list mimicking DB table
        private static readonly List<Student> _students = new List<Student>();
        private static readonly object _lock = new object();

        public IEnumerable<Student> GetAll()
        {
            lock (_lock)
            {
                // Return a copy to avoid external modification issues
                return _students.ToList();
            }
        }

        public Student? GetById(int id)
        {
            lock (_lock)
            {
                return _students.FirstOrDefault(s => s.Id == id);
            }
        }

        public void Add(Student entity)
        {
            lock (_lock)
            {
                if (entity.Id <= 0)
                {
                    // Simple auto-increment
                    entity.Id = _students.Count > 0 ? _students.Max(s => s.Id) + 1 : 1;
                }
                
                if (_students.Any(s => s.Id == entity.Id))
                {
                    throw new InvalidOperationException($"Student with Id {entity.Id} already exists.");
                }

                _students.Add(entity);
                Logger.Instance.Log($"[Repo - Student] Added student: '{entity.Name}' (ID: {entity.Id})");
            }
        }

        public void Update(Student entity)
        {
            lock (_lock)
            {
                var existing = _students.FirstOrDefault(s => s.Id == entity.Id);
                if (existing == null)
                {
                    throw new KeyNotFoundException($"Student with Id {entity.Id} not found.");
                }

                existing.Name = entity.Name;
                existing.Email = entity.Email;
                Logger.Instance.Log($"[Repo - Student] Updated student: ID {entity.Id}");
            }
        }

        public void Delete(int id)
        {
            lock (_lock)
            {
                var existing = _students.FirstOrDefault(s => s.Id == id);
                if (existing != null)
                {
                    _students.Remove(existing);
                    Logger.Instance.Log($"[Repo - Student] Deleted student: ID {id}");
                }
            }
        }

        /// <summary>
        /// Clears all static in-memory data. Useful for unit testing isolation.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _students.Clear();
            }
        }
    }

    /// <summary>
    /// In-memory repository for Course records.
    /// </summary>
    public class CourseRepository : IRepository<Course>
    {
        private static readonly List<Course> _courses = new List<Course>();
        private static readonly object _lock = new object();

        public IEnumerable<Course> GetAll()
        {
            lock (_lock)
            {
                return _courses.ToList();
            }
        }

        public Course? GetById(int id)
        {
            lock (_lock)
            {
                return _courses.FirstOrDefault(c => c.Id == id);
            }
        }

        public void Add(Course entity)
        {
            lock (_lock)
            {
                if (entity.Id <= 0)
                {
                    entity.Id = _courses.Count > 0 ? _courses.Max(c => c.Id) + 1 : 1;
                }

                if (_courses.Any(c => c.Id == entity.Id))
                {
                    throw new InvalidOperationException($"Course with Id {entity.Id} already exists.");
                }

                _courses.Add(entity);
                Logger.Instance.Log($"[Repo - Course] Added course: '{entity.Title}' (ID: {entity.Id})");
            }
        }

        public void Update(Course entity)
        {
            lock (_lock)
            {
                var existing = _courses.FirstOrDefault(c => c.Id == entity.Id);
                if (existing == null)
                {
                    throw new KeyNotFoundException($"Course with Id {entity.Id} not found.");
                }

                existing.Title = entity.Title;
                existing.Code = entity.Code;
                Logger.Instance.Log($"[Repo - Course] Updated course: ID {entity.Id}");
            }
        }

        public void Delete(int id)
        {
            lock (_lock)
            {
                var existing = _courses.FirstOrDefault(c => c.Id == id);
                if (existing != null)
                {
                    _courses.Remove(existing);
                    Logger.Instance.Log($"[Repo - Course] Deleted course: ID {id}");
                }
            }
        }

        /// <summary>
        /// Clears all static in-memory data. Useful for unit testing isolation.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _courses.Clear();
            }
        }
    }

    #endregion

    #region Unit of Work

    /// <summary>
    /// IUnitOfWork coordinates transactions and groups repository calls.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Student> Students { get; }
        IRepository<Course> Courses { get; }
        void Save();
    }

    /// <summary>
    /// In-memory Unit of Work implementation.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed = false;

        public IRepository<Student> Students { get; }
        public IRepository<Course> Courses { get; }

        public UnitOfWork()
        {
            // Instantiating concrete in-memory repositories
            Students = new StudentRepository();
            Courses = new CourseRepository();
            Logger.Instance.Log("UnitOfWork transaction context initialized.");
        }

        /// <summary>
        /// Saves all pending modifications. In an ORM, this maps to DB SaveChanges() transaction.
        /// </summary>
        public void Save()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(UnitOfWork), "Cannot execute Save on a disposed UnitOfWork.");
            }

            Logger.Instance.Log("[UnitOfWork] Save() called. Transaction successfully committed to the in-memory data store.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clean up DB context contexts here if using Entity Framework etc.
                    Logger.Instance.Log("Disposing UnitOfWork transaction context.");
                }
                _disposed = true;
            }
        }
    }

    #endregion
}
