using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Results;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Services
{
    //TODO remove this class and use the repository directly in the controller
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

        public async Task<ResultWith<StudentDto>> PatchStudent(int sId, PatchStudentDto student, byte[] rowVersion)
        {
             return await _studentRepository.PatchStudent(sId, student, rowVersion);
        }

        public async Task<PagedList<StudentDto>> GetStudents(GetStudentsQueryDto getStudentsQueryDto)
        {
            return await _studentRepository.GetStudents(getStudentsQueryDto);
        }
    }
}
