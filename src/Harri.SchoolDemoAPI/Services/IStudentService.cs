using Harri.SchoolDemoAPI.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Services
{
    public interface IStudentService
    {
        Task<int> AddStudent(NewStudentDto newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<StudentDto?> GetStudent(int sId);
        Task<StudentDto?> PatchStudent(int sId, StudentPatchDto student);
        Task<bool> UpdateStudent(int sId, UpdateStudentDto student);

        Task<List<StudentDto>> GetAllStudents();
        Task<List<StudentDto>> QueryStudents(string? name, GPAQueryDto? gpaQuery);
    }
}