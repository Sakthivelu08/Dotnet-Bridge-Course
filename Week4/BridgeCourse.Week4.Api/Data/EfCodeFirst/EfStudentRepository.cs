using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.EfCodeFirst
{
    public class EfStudentRepository : IRepository<Student>
    {
        private readonly EfCodeFirstDbContext _context;

        public EfStudentRepository(EfCodeFirstDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Student> GetAll()
        {
            return _context.Students.AsNoTracking().ToList();
        }

        public Student? GetById(int id)
        {
            return _context.Students.Find(id);
        }

        public void Add(Student entity)
        {
            _context.Students.Add(entity);
        }

        public void Update(Student entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
        }
    }
}
