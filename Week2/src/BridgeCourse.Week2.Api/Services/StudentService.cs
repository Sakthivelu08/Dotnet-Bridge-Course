using System;
using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week2.Api.DTOs;
using BridgeCourse.Week2.Api.Models;
using BridgeCourse.Week2.Api.Repositories;
using BridgeCourse.Week2.Api.Strategies;

namespace BridgeCourse.Week2.Api.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GradeStrategyFactory _gradeStrategyFactory;

        public StudentService(IUnitOfWork unitOfWork, GradeStrategyFactory gradeStrategyFactory)
        {
            _unitOfWork = unitOfWork;
            _gradeStrategyFactory = gradeStrategyFactory;
        }

        public IEnumerable<StudentReadDto> GetAll()
        {
            var strategy = _gradeStrategyFactory.GetStrategy();
            return _unitOfWork.Students.GetAll().Select(s => MapToReadDto(s, strategy));
        }

        public StudentReadDto? GetById(int id)
        {
            var student = _unitOfWork.Students.GetById(id);
            if (student == null) return null;

            var strategy = _gradeStrategyFactory.GetStrategy();
            return MapToReadDto(student, strategy);
        }

        public StudentReadDto Create(StudentCreateDto createDto)
        {
            var student = new Student
            {
                Name = createDto.Name,
                Age = createDto.Age,
                Email = createDto.Email,
                Grade = createDto.Grade,
                InternalNotes = createDto.InternalNotes
            };

            _unitOfWork.Students.Add(student);
            _unitOfWork.Save();

            var strategy = _gradeStrategyFactory.GetStrategy();
            return MapToReadDto(student, strategy);
        }

        public StudentReadDto? Update(int id, StudentCreateDto updateDto)
        {
            var student = _unitOfWork.Students.GetById(id);
            if (student == null) return null;

            student.Name = updateDto.Name;
            student.Age = updateDto.Age;
            student.Email = updateDto.Email;
            student.Grade = updateDto.Grade;
            student.InternalNotes = updateDto.InternalNotes;

            _unitOfWork.Students.Update(student);
            _unitOfWork.Save();

            var strategy = _gradeStrategyFactory.GetStrategy();
            return MapToReadDto(student, strategy);
        }

        public bool Delete(int id)
        {
            var student = _unitOfWork.Students.GetById(id);
            if (student == null) return false;

            _unitOfWork.Students.Delete(id);
            _unitOfWork.Save();
            return true;
        }

        public IEnumerable<StudentReadDto> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GetAll();
            }

            var strategy = _gradeStrategyFactory.GetStrategy();
            var matches = _unitOfWork.Students.GetAll()
                .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            return matches.Select(s => MapToReadDto(s, strategy));
        }

        private StudentReadDto MapToReadDto(Student student, IGradeStrategy strategy)
        {
            return new StudentReadDto
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age,
                Email = student.Email,
                RawGrade = student.Grade,
                FormattedGrade = strategy.FormatGrade(student.Grade)
            };
        }
    }
}
