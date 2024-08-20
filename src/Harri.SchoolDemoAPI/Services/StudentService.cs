using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<int> AddStudent(NewStudentDto newStudent)
        {
            return await _studentRepository.AddStudent(newStudent);
        }

        public async Task<StudentDto?> GetStudent(int sId)
        {
            return await _studentRepository.GetStudent(sId);
        }


        public async Task<bool> UpdateStudent(int sId, UpdateStudentDto student)
        {
            return await _studentRepository.UpdateStudent(sId, student);
        }

        public async Task<bool?> DeleteStudent(int sId)
        {
            return await _studentRepository.DeleteStudent(sId);
        }

        public async Task<StudentDto?> PatchStudent(int sId, StudentPatchDto student)
        {
            var existingStudent = await _studentRepository.GetStudent(sId);
            if (existingStudent is not null)
            {
                student.ApplyChangesTo(existingStudent);
                await _studentRepository.UpdateStudent(sId, existingStudent.AsUpdateStudentDto());
                return existingStudent;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<StudentDto>> GetStudents(string? name = null, GPAQueryDto? gpaQuery = null, SortOrder? orderBy = null)
        {
            return await _studentRepository.GetStudents(name, gpaQuery, orderBy);
        }
    }
}
