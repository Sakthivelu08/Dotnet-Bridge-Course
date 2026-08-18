using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week4.Api.DTOs;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;

namespace BridgeCourse.Week4.Api.Services
{
    public class TeacherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeacherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TeacherReadDto> GetAllTeachers()
        {
            return _unitOfWork.Teachers.GetAll().Select(MapToReadDto);
        }

        public TeacherReadDto? GetTeacherById(int id)
        {
            var teacher = _unitOfWork.Teachers.GetById(id);
            return teacher == null ? null : MapToReadDto(teacher);
        }

        public TeacherReadDto CreateTeacher(TeacherCreateDto createDto)
        {
            var teacher = new Teacher
            {
                Name = createDto.Name,
                Email = createDto.Email,
                Subject = createDto.Subject,
                Salary = createDto.Salary,
                InternalNotes = createDto.InternalNotes
            };

            _unitOfWork.Teachers.Add(teacher);
            _unitOfWork.Save();

            return MapToReadDto(teacher);
        }

        public bool UpdateTeacher(int id, TeacherCreateDto updateDto)
        {
            var teacher = _unitOfWork.Teachers.GetById(id);
            if (teacher == null) return false;

            teacher.Name = updateDto.Name;
            teacher.Email = updateDto.Email;
            teacher.Subject = updateDto.Subject;
            teacher.Salary = updateDto.Salary;
            teacher.InternalNotes = updateDto.InternalNotes;

            _unitOfWork.Teachers.Update(teacher);
            _unitOfWork.Save();

            return true;
        }

        public bool DeleteTeacher(int id)
        {
            var teacher = _unitOfWork.Teachers.GetById(id);
            if (teacher == null) return false;

            _unitOfWork.Teachers.Delete(id);
            _unitOfWork.Save();

            return true;
        }

        private TeacherReadDto MapToReadDto(Teacher teacher)
        {
            return new TeacherReadDto
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Email = teacher.Email,
                Subject = teacher.Subject,
                Salary = teacher.Salary
            };
        }
    }
}
