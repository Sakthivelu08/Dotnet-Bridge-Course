using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Data.EfCodeFirst
{
    public class EfTeacherRepository : IRepository<Teacher>
    {
        private readonly EfCodeFirstDbContext _context;

        public EfTeacherRepository(EfCodeFirstDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Teacher> GetAll()
        {
            return _context.Teachers.AsNoTracking().ToList();
        }

        public Teacher? GetById(int id)
        {
            return _context.Teachers.Find(id);
        }

        public void Add(Teacher entity)
        {
            _context.Teachers.Add(entity);
        }

        public void Update(Teacher entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var teacher = _context.Teachers.Find(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }
        }
    }
}
