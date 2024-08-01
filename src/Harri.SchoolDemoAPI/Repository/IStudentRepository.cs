using Harri.SchoolDemoAPI.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Repository
{
    public interface IStudentRepository
    {
        Task<int> AddStudent(NewStudentDto newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<StudentDto?> GetStudent(int sId);
        Task<bool> UpdateStudent(int sId, UpdateStudentDto newStudent);

        Task<List<StudentDto>> GetStudents(string? name = null, GPAQueryDto? GPAQuery = null);
    }
}