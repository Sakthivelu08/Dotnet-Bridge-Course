using System.Collections.Generic;
using BridgeCourse.Week2.Api.DTOs;

namespace BridgeCourse.Week2.Api.Services
{
    public interface IStudentService
    {
        IEnumerable<StudentReadDto> GetAll();
        StudentReadDto? GetById(int id);
        StudentReadDto Create(StudentCreateDto createDto);
        StudentReadDto? Update(int id, StudentCreateDto updateDto);
        bool Delete(int id);
        IEnumerable<StudentReadDto> SearchByName(string name);
    }
}
