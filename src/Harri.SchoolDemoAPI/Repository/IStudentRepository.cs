using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Results;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Repository
{
    public interface IStudentRepository
    {
        Task<int> AddStudent(NewStudentDto newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<StudentDto?> GetStudent(int sId);
        Task<Result> UpdateStudent(int sId, UpdateStudentDto newStudent);

        Task<PagedList<StudentDto>> GetStudents(GetStudentsQueryDto getStudentsQueryDto);
    }
}