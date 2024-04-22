using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Repository
{
    public interface IStudentRepository
    {
        Task<int> AddStudent(NewStudent newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<Student?> GetStudent(int sId);
        Task<bool> UpdateStudent(Student newStudent);
    }
}