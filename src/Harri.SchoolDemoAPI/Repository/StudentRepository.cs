using Harri.SchoolDemoAPI.Models;
using System.ComponentModel;

namespace Harri.SchoolDemoAPI.Repository
{
    public class StudentRepository : IStudentRepository
    {
        public StudentRepository() { }
        public int AddStudent(NewStudent newStudent)
        {
            throw new System.NotImplementedException();
            return 1234;
        }

        public Student GetStudent(int sId)
        {
            throw new System.NotImplementedException();
            return new Student() { SId= 1234, Name = "Test", GPA = 3.91m};
        }


        public void UpdateStudent(Student newStudent)
        {

        }
        public void DeleteStudent(int id)
        {

        }
    }
}
