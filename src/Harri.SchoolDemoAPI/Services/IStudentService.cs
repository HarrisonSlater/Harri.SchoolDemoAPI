using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Services
{
    public interface IStudentService
    {
        Task<int> AddStudent(NewStudent newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<Student?> GetStudent(int sId);
        Task<Student?> PatchStudent(int sId, StudentPatchDto student);
        Task<bool> UpdateStudent(Student student);
    }
}