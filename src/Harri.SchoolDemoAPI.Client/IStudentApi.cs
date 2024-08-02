using Harri.SchoolDemoAPI.Models.Dto;
using RestSharp;

namespace Harri.SchoolDemoAPI.Client
{
    public interface IStudentApi
    {
        Task<int?> AddStudent(NewStudentDto student);
        Task<RestResponse<int?>> AddStudentRestResponse(NewStudentDto student);
        Task<bool> DeleteStudent(int sId);
        Task<RestResponse> DeleteStudentRestResponse(int sId);
        Task<StudentDto?> GetStudent(int sId);
        Task<RestResponse<StudentDto>> GetStudentRestResponse(int sId);
        Task<List<StudentDto>?> GetStudents(string? name = null, GPAQueryDto? gpaQuery = null);
        Task<RestResponse<List<StudentDto>>> GetStudentsRestResponse(string? name = null, GPAQueryDto? gpaQuery = null);
        Task<StudentDto?> PatchStudent(int sId, StudentPatchDto student);
        Task<RestResponse<StudentDto?>> PatchStudentRestResponse(int sId, StudentPatchDto student);
        Task<bool?> UpdateStudent(int sId, UpdateStudentDto student);
        Task<RestResponse<bool?>> UpdateStudentRestResponse(int sId, UpdateStudentDto student);
    }
}