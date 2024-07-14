using Harri.SchoolDemoAPI.Models.Dto;
using RestSharp;

namespace Harri.SchoolDemoApi.Client
{
    public interface IStudentApiClient
    {
        Task<int?> AddStudent(NewStudentDto student);
        Task<RestResponse<int?>> AddStudentRestResponse(NewStudentDto student);
        Task<bool> DeleteStudent(int sId);
        Task<RestResponse> DeleteStudentRestResponse(int sId);
        Task<StudentDto?> GetStudent(int sId);
        Task<RestResponse<StudentDto>> GetStudentRestResponse(int sId);
        Task<List<StudentDto>?> GetStudents();
        Task<RestResponse<List<StudentDto>>> GetStudentsRestResponse();
        Task<StudentDto?> PatchStudent(int sId, StudentPatchDto student);
        Task<RestResponse<StudentDto?>> PatchStudentRestResponse(int sId, StudentPatchDto student);
        Task<List<StudentDto>?> QueryStudents(string? name = null, GPAQueryDto? gpaQuery = null);
        Task<RestResponse<List<StudentDto>>> QueryStudentsRestResponse(string? name = null, GPAQueryDto? gpaQuery = null);
        Task<bool?> UpdateStudent(int sId, UpdateStudentDto student);
        Task<RestResponse<bool?>> UpdateStudentRestResponse(int sId, UpdateStudentDto student);
    }
}