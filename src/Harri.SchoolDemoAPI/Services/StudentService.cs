using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Results;
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

        public async Task<Result> UpdateStudent(int sId, UpdateStudentDto student, byte[] rowVersion)
        {
            return await _studentRepository.UpdateStudent(sId, student, rowVersion);
        }

        public async Task<bool?> DeleteStudent(int sId)
        {
            return await _studentRepository.DeleteStudent(sId);
        }

        public async Task<StudentDto?> PatchStudent(int sId, StudentPatchDto student, byte[] rowVersion)
        {
            var existingStudent = await _studentRepository.GetStudent(sId);
            if (existingStudent is not null)
            {
                student.ApplyChangesTo(existingStudent);
                await _studentRepository.UpdateStudent(sId, existingStudent.AsUpdateStudentDto(), rowVersion);
                return existingStudent;
            }
            else
            {
                return null;
            }
        }

        public async Task<PagedList<StudentDto>> GetStudents(GetStudentsQueryDto getStudentsQueryDto)
        {
            return await _studentRepository.GetStudents(getStudentsQueryDto);
        }
    }
}
