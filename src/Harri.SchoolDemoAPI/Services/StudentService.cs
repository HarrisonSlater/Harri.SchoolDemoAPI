using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Repository;
using System.Data;

namespace Harri.SchoolDemoAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public int AddStudent(NewStudent newStudent)
        {
            return _studentRepository.AddStudent(newStudent);
        }

        public Student? GetStudent(int sId)
        {
            return _studentRepository.GetStudent(sId);
        }


        public bool UpdateStudent(Student student)
        {
            return _studentRepository.UpdateStudent(student);
        }

        public bool? DeleteStudent(int sId)
        {
            return _studentRepository.DeleteStudent(sId);
        }

        public Student? PatchStudent(int sId, StudentPatchDto student)
        {
            var existingStudent = _studentRepository.GetStudent(sId);
            if (existingStudent != null)
            {
                student.ApplyChangesTo(existingStudent);
                _studentRepository.UpdateStudent(existingStudent);
                return existingStudent;
            }
            else
            {
                return null;
            }
        }
    }
}
