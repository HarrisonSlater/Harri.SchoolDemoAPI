using Harri.SchoolDemoAPI.Models;

namespace Harri.SchoolDemoAPI.Repository
{
    public interface IStudentRepository
    {
        int AddStudent(NewStudent newStudent);
        bool DeleteStudent(int id);
        Student? GetStudent(int sId);
        bool UpdateStudent(Student newStudent);
    }
}