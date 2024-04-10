using Harri.SchoolDemoAPI.Models;
using RestSharp;

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
        public async Task<int?> AddStudent(NewStudent student)
        {
            var restResponse = await AddStudentRestResponse(student);
            return restResponse.Data;
        }

        public async Task<RestResponse<int?>> AddStudentRestResponse(NewStudent student)
        {
            var request = new RestRequest("student/").AddBody(student);
            var restResponse = await _restClient.ExecutePostAsync<int?>(request);
            return restResponse;
        }

        // Get
        public async Task<Student?> GetStudent(int sId)
        {
            var restResponse = await GetStudentRestResponse(sId);
            return restResponse.Data;
        }

        public async Task<RestResponse<Student>> GetStudentRestResponse(int sId)
        {
            var request = new RestRequest("student/{sId}").AddUrlSegment("sId", sId);
            var restResponse = await _restClient.ExecuteGetAsync<Student>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }

        // Update
        public async Task<bool> UpdateStudent(Student student)
        {
            var restResponse = await UpdateStudentRestResponse(student);
            return restResponse.IsSuccessful;
        }

        public async Task<RestResponse> UpdateStudentRestResponse(Student student)
        {
            var request = new RestRequest("student/").AddBody(student);
            var restResponse = await _restClient.ExecutePutAsync(request);
            return restResponse;
        }

        // Delete

        public async Task<bool> DeleteStudent(int sId)
        {
            var restResponse = await DeleteStudentRestResponse(sId);
            return restResponse.IsSuccessful;
        }

        public async Task<RestResponse> DeleteStudentRestResponse(int sId)
        {
            var request = new RestRequest("student/{sId}").AddUrlSegment("sId", sId);
            request.Method = Method.Delete;

            var restResponse = await _restClient.ExecuteAsync(request);
            return restResponse;
        }
    }
}
