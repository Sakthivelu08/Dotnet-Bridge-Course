using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;
using DbTeacher = BridgeCourse.Week4.Api.Data.EfDbFirst.Models.Teacher;

namespace BridgeCourse.Week4.Api.Data.EfDbFirst
{
    public class DbFirstTeacherRepository : IRepository<Teacher>
    {
        private readonly EfDbFirstDbContext _context;

        public DbFirstTeacherRepository(EfDbFirstDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Teacher> GetAll()
        {
            return _context.Teachers.AsNoTracking().ToList().Select(MapToDomain);
        }

        public Teacher? GetById(int id)
        {
            var dbTeacher = _context.Teachers.Find(id);
            return dbTeacher == null ? null : MapToDomain(dbTeacher);
        }

        public void Add(Teacher entity)
        {
            var dbTeacher = MapToDb(entity);
            _context.Teachers.Add(dbTeacher);
            _context.SaveChanges();
            entity.Id = dbTeacher.Id; // Sync ID
        }

        public void Update(Teacher entity)
        {
            var dbTeacher = _context.Teachers.Find(entity.Id);
            if (dbTeacher != null)
            {
                dbTeacher.Name = entity.Name;
                dbTeacher.Email = entity.Email;
                dbTeacher.Subject = entity.Subject;
                dbTeacher.Salary = entity.Salary;
                dbTeacher.InternalNotes = entity.InternalNotes;

                _context.Entry(dbTeacher).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var dbTeacher = _context.Teachers.Find(id);
            if (dbTeacher != null)
            {
                _context.Teachers.Remove(dbTeacher);
            }
        }

        private Teacher MapToDomain(DbTeacher t)
        {
            return new Teacher
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Subject = t.Subject,
                Salary = t.Salary,
                InternalNotes = t.InternalNotes
            };
        }

        private DbTeacher MapToDb(Teacher t)
        {
            return new DbTeacher
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Subject = t.Subject,
                Salary = t.Salary,
                InternalNotes = t.InternalNotes
            };
        }
    }
}
