using Harri.SchoolDemoAPI.Models.Dto;
using RestSharp;
using System.Security.Cryptography;

namespace Harri.SchoolDemoApi.Client
{
    public class StudentApiClient
    {
        private const string BaseRoute = "students/";
        private readonly RestClient _restClient;

        public StudentApiClient(string? uri)
        {
            var options = new RestClientOptions(uri is null ? "https://localhost:44301/" : uri);
            _restClient = new RestClient(options);
        }

        // Add
        public async Task<int?> AddStudent(NewStudentDto student)
        {
            var restResponse = await AddStudentRestResponse(student);
            return restResponse.Data;
        }

        public async Task<RestResponse<int?>> AddStudentRestResponse(NewStudentDto student)
        {
            var request = new RestRequest(BaseRoute).AddBody(student);
            var restResponse = await _restClient.ExecutePostAsync<int?>(request);
            return restResponse;
        }

        // Get
        public async Task<StudentDto?> GetStudent(int sId)
        {
            var restResponse = await GetStudentRestResponse(sId);
            return restResponse.Data;
        }

        public async Task<RestResponse<StudentDto>> GetStudentRestResponse(int sId)
        {
            var request = new RestRequest(BaseRoute + "{sId}").AddUrlSegment("sId", sId);
            var restResponse = await _restClient.ExecuteGetAsync<StudentDto>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }

        // Get
        public async Task<List<StudentDto>?> GetStudents()
        {
            var restResponse = await GetStudentsRestResponse();
            return restResponse.Data;
        }

        public async Task<RestResponse<List<StudentDto>>> GetStudentsRestResponse()
        {
            var request = new RestRequest(BaseRoute);
            var restResponse = await _restClient.ExecuteGetAsync<List<StudentDto>>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }

        // Update
        public async Task<bool?> UpdateStudent(int sId, UpdateStudentDto student)
        {
            var restResponse = await UpdateStudentRestResponse(sId, student);
            return restResponse.Data;
        }

        public async Task<RestResponse<bool?>> UpdateStudentRestResponse(int sId, UpdateStudentDto student)
        {
            var request = new RestRequest(BaseRoute + "{sId}").AddUrlSegment("sId", sId).AddBody(student);
            var restResponse = await _restClient.ExecutePutAsync<bool?>(request);
            return restResponse;
        }

        // Delete

        public async Task<bool> DeleteStudent(int sId)
        {
            var restResponse = await DeleteStudentRestResponse(sId);
            return restResponse.Data;
        }

        public async Task<RestResponse<bool>> DeleteStudentRestResponse(int sId)
        {
            var request = new RestRequest(BaseRoute + "{sId}").AddUrlSegment("sId", sId);
            request.Method = Method.Delete;

            var restResponse = await _restClient.ExecuteAsync<bool>(request);
            return restResponse;
        }

        // Patch
        public async Task<StudentDto?> PatchStudent(int sId, StudentPatchDto student)
        {
            var restResponse = await PatchStudentRestResponse(sId, student);
            return restResponse.Data;
        }

        public async Task<RestResponse<StudentDto?>> PatchStudentRestResponse(int sId, StudentPatchDto student)
        {
            var request = new RestRequest(BaseRoute + "{sId}").AddUrlSegment("sId", sId).AddBody(student.GetObjectToSerialize());
            request.Method = Method.Patch;

            var restResponse = await _restClient.ExecuteAsync<StudentDto?>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }
    }
}
