using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using RestSharp;
using System.Security.Cryptography;

namespace Harri.SchoolDemoApi.Client
{
    public class StudentApiClient
    {
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
            var request = new RestRequest("student/").AddBody(student);
            var restResponse = await _restClient.ExecutePostAsync<int?>(request);
            return restResponse;
        }

        // Get
        public async Task<SchoolDemoAPI.Models.StudentDto?> GetStudent(int sId)
        {
            var restResponse = await GetStudentRestResponse(sId);
            return restResponse.Data;
        }

        public async Task<RestResponse<SchoolDemoAPI.Models.StudentDto>> GetStudentRestResponse(int sId)
        {
            var request = new RestRequest("student/{sId}").AddUrlSegment("sId", sId);
            var restResponse = await _restClient.ExecuteGetAsync<SchoolDemoAPI.Models.StudentDto>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }

        // Update
        public async Task<bool?> UpdateStudent(SchoolDemoAPI.Models.StudentDto student)
        {
            var restResponse = await UpdateStudentRestResponse(student);
            return restResponse.Data;
        }

        public async Task<RestResponse<bool?>> UpdateStudentRestResponse(SchoolDemoAPI.Models.StudentDto student)
        {
            var request = new RestRequest("student/").AddBody(student);
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
            var request = new RestRequest("student/{sId}").AddUrlSegment("sId", sId);
            request.Method = Method.Delete;

            var restResponse = await _restClient.ExecuteAsync<bool>(request);
            return restResponse;
        }

        // Patch
        public async Task<SchoolDemoAPI.Models.StudentDto?> PatchStudent(int sId, StudentPatchDto student)
        {
            var restResponse = await PatchStudentRestResponse(sId, student);
            return restResponse.Data;
        }

        public async Task<RestResponse<SchoolDemoAPI.Models.StudentDto?>> PatchStudentRestResponse(int sId, StudentPatchDto student)
        {
            var request = new RestRequest("students/{sId}").AddUrlSegment("sId", sId).AddBody(student.GetObjectToSerialize());
            request.Method = Method.Patch;

            var restResponse = await _restClient.ExecuteAsync<SchoolDemoAPI.Models.StudentDto?>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }
    }
}
