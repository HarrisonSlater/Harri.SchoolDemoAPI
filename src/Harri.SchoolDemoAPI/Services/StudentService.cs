using Harri.SchoolDemoAPI.Models;
using System.ComponentModel;

namespace Harri.SchoolDemoAPI.Services
{
    public class StudentService : IStudentService
    {
        public StudentService() { }
        public int AddStudent(NewStudent newStudent)
        {
            return 1234;
        }

        public Student GetStudent(int sId)
        {
            return new Student() { Name = "Test" };
        }


        public void UpdateStudent(Student newStudent)
        {

        }
        public void DeleteStudent(int id)
        {

        }
    }
}
