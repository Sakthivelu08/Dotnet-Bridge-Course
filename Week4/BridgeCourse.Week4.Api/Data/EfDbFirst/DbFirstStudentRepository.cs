using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;
using DbStudent = BridgeCourse.Week4.Api.Data.EfDbFirst.Models.Student;

namespace BridgeCourse.Week4.Api.Data.EfDbFirst
{
    public class DbFirstStudentRepository : IRepository<Student>
    {
        private readonly EfDbFirstDbContext _context;

        public DbFirstStudentRepository(EfDbFirstDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Student> GetAll()
        {
            return _context.Students.AsNoTracking().ToList().Select(MapToDomain);
        }

        public Student? GetById(int id)
        {
            var dbStudent = _context.Students.Find(id);
            return dbStudent == null ? null : MapToDomain(dbStudent);
        }

        public void Add(Student entity)
        {
            var dbStudent = MapToDb(entity);
            _context.Students.Add(dbStudent);
            _context.SaveChanges();
            entity.Id = dbStudent.Id; // Sync ID back
        }

        public void Update(Student entity)
        {
            var dbStudent = _context.Students.Find(entity.Id);
            if (dbStudent != null)
            {
                dbStudent.Name = entity.Name;
                dbStudent.Age = entity.Age;
                dbStudent.Email = entity.Email;
                dbStudent.Grade = entity.Grade;
                dbStudent.InternalNotes = entity.InternalNotes;
                dbStudent.EnrolledOn = entity.EnrolledOn;

                _context.Entry(dbStudent).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var dbStudent = _context.Students.Find(id);
            if (dbStudent != null)
            {
                _context.Students.Remove(dbStudent);
            }
        }

        private Student MapToDomain(DbStudent s)
        {
            return new Student
            {
                Id = s.Id,
                Name = s.Name,
                Age = s.Age,
                Email = s.Email,
                Grade = s.Grade,
                InternalNotes = s.InternalNotes,
                EnrolledOn = s.EnrolledOn
            };
        }

        private DbStudent MapToDb(Student s)
        {
            return new DbStudent
            {
                Id = s.Id,
                Name = s.Name,
                Age = s.Age,
                Email = s.Email,
                Grade = s.Grade,
                InternalNotes = s.InternalNotes,
                EnrolledOn = s.EnrolledOn
            };
        }
    }
}
