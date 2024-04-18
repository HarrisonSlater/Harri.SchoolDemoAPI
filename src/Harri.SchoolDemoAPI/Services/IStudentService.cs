using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;

namespace Harri.SchoolDemoAPI.Services
{
    public interface IStudentService
    {
        int AddStudent(NewStudent newStudent);
        bool? DeleteStudent(int sId);
        Student? GetStudent(int sId);
        Student? PatchStudent(int sId, StudentPatchDto student);
        bool UpdateStudent(Student student);
    }
}