using System;
using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week2.Api.DTOs;
using BridgeCourse.Week2.Api.Models;
using BridgeCourse.Week2.Api.Repositories;

namespace BridgeCourse.Week2.Api.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeacherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TeacherReadDto> GetAll()
        {
            return _unitOfWork.Teachers.GetAll().Select(MapToReadDto);
        }

        public TeacherReadDto? GetById(int id)
        {
            var teacher = _unitOfWork.Teachers.GetById(id);
            if (teacher == null) return null;
            return MapToReadDto(teacher);
        }

        public TeacherReadDto Create(TeacherCreateDto createDto)
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

        public TeacherReadDto? Update(int id, TeacherCreateDto updateDto)
        {
            var teacher = _unitOfWork.Teachers.GetById(id);
            if (teacher == null) return null;

            teacher.Name = updateDto.Name;
            teacher.Email = updateDto.Email;
            teacher.Subject = updateDto.Subject;
            teacher.Salary = updateDto.Salary;
            teacher.InternalNotes = updateDto.InternalNotes;

            _unitOfWork.Teachers.Update(teacher);
            _unitOfWork.Save();

            return MapToReadDto(teacher);
        }

        public bool Delete(int id)
        {
            var teacher = _unitOfWork.Teachers.GetById(id);
            if (teacher == null) return false;

            _unitOfWork.Teachers.Delete(id);
            _unitOfWork.Save();
            return true;
        }

        public IEnumerable<TeacherReadDto> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GetAll();
            }

            var matches = _unitOfWork.Teachers.GetAll()
                .Where(t => t.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            return matches.Select(MapToReadDto);
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
