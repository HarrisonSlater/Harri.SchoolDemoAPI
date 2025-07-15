using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Services
{
    public interface IStudentService
    {
        Task<int> AddStudent(NewStudentDto newStudent);
        Task<bool?> DeleteStudent(int sId);
        Task<StudentDto?> GetStudent(int sId);
        Task<ResultWith<StudentDto>> PatchStudent(int sId, StudentPatchDto student, byte[] rowVersion);
        Task<Result> UpdateStudent(int sId, UpdateStudentDto student, byte[] rowVersion);

        Task<PagedList<StudentDto>> GetStudents(GetStudentsQueryDto getStudentsQueryDto);
    }
}