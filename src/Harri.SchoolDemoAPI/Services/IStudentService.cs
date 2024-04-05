using Harri.SchoolDemoAPI.Models;

namespace Harri.SchoolDemoAPI.Services
{
    public interface IStudentService
    {
        int AddStudent(NewStudent newStudent);
        void DeleteStudent(int id);
        Student GetStudent(int sId);
        void UpdateStudent(Student newStudent);
    }
}