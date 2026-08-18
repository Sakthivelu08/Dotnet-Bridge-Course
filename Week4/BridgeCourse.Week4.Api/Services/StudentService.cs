using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week4.Api.DTOs;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Services
{
    public class StudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<StudentReadDto> GetAllStudents()
        {
            return _unitOfWork.Students.GetAll().Select(MapToReadDto);
        }

        public StudentReadDto? GetStudentById(int id)
        {
            var student = _unitOfWork.Students.GetById(id);
            return student == null ? null : MapToReadDto(student);
        }

        public StudentReadDto CreateStudent(StudentCreateDto createDto)
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

            return MapToReadDto(student);
        }

        public bool UpdateStudent(int id, StudentCreateDto updateDto)
        {
            var student = _unitOfWork.Students.GetById(id);
            if (student == null) return false;

            student.Name = updateDto.Name;
            student.Age = updateDto.Age;
            student.Email = updateDto.Email;
            student.Grade = updateDto.Grade;
            student.InternalNotes = updateDto.InternalNotes;

            _unitOfWork.Students.Update(student);
            _unitOfWork.Save();

            return true;
        }

        public bool DeleteStudent(int id)
        {
            var student = _unitOfWork.Students.GetById(id);
            if (student == null) return false;

            _unitOfWork.Students.Delete(id);
            _unitOfWork.Save();

            return true;
        }

        private StudentReadDto MapToReadDto(Student student)
        {
            return new StudentReadDto
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age,
                Email = student.Email,
                Grade = student.Grade,
                EnrolledOn = student.EnrolledOn
            };
        }
    }
}
