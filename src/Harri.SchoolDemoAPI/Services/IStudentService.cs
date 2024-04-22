using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Services
{
    public interface IStudentService
    {
        Task<int> AddStudent(NewStudentDto newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<Models.StudentDto?> GetStudent(int sId);
        Task<Models.StudentDto?> PatchStudent(int sId, StudentPatchDto student);
        Task<bool> UpdateStudent(Models.StudentDto student);
    }
}