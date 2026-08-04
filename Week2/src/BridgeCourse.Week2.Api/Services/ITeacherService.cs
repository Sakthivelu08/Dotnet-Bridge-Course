using System.Collections.Generic;
using BridgeCourse.Week2.Api.DTOs;

namespace BridgeCourse.Week2.Api.Services
{
    public interface ITeacherService
    {
        IEnumerable<TeacherReadDto> GetAll();
        TeacherReadDto? GetById(int id);
        TeacherReadDto Create(TeacherCreateDto createDto);
        TeacherReadDto? Update(int id, TeacherCreateDto updateDto);
        bool Delete(int id);
        IEnumerable<TeacherReadDto> SearchByName(string name);
    }
}
