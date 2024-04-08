using Harri.SchoolDemoAPI.Models;

namespace Harri.SchoolDemoAPI.Repository
{
    public interface IStudentRepository
    {
        int AddStudent(NewStudent newStudent);
        void DeleteStudent(int id);
        Student GetStudent(int sId);
        void UpdateStudent(Student newStudent);
    }
}