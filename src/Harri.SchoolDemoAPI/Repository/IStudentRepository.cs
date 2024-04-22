using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Repository
{
    public interface IStudentRepository
    {
        Task<int> AddStudent(NewStudentDto newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<Models.StudentDto?> GetStudent(int sId);
        Task<bool> UpdateStudent(Models.StudentDto newStudent);
    }
}