﻿using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Microsoft.Net.Http.Headers;
using RestSharp;

namespace Harri.SchoolDemoAPI.Client
{
    /// <summary>
    /// .NET 8 REST Client for Harri.SchoolDemoAPI using RestSharp.
    /// 
    /// Does not throw exceptions on failed requests.
    /// </summary>
    public class StudentApiClient : IStudentApi
    {
        private const string BaseRoute = "students/";
        private readonly RestClient _restClient;

        public StudentApiClient(string uri)
        {
            var options = new RestClientOptions(uri);
            _restClient = new RestClient(options);
        }

        public StudentApiClient(HttpClient httpClient)
        {
            _restClient = new RestClient(httpClient);
        }

        public StudentApiClient(RestClient restClient)
        {
            _restClient = restClient;
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
            SetRowVersionFromETagHeader(restResponse);

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

        // Update
        public async Task<bool> UpdateStudent(int sId, UpdateStudentDto student, byte[] rowVersion)
        {
            var restResponse = await UpdateStudentRestResponse(sId, student, rowVersion);
            return restResponse.IsSuccessStatusCode;
        }

        public async Task<RestResponse> UpdateStudentRestResponse(int sId, UpdateStudentDto student, byte[] rowVersion)
        {
            var request = new RestRequest(BaseRoute + "{sId}").AddUrlSegment("sId", sId).AddBody(student);
            SetIfMatchHeader(request, rowVersion);

            var restResponse = await _restClient.ExecutePutAsync(request);
            return restResponse;
        }

        // Delete
        public async Task<bool> DeleteStudent(int sId)
        {
            var restResponse = await DeleteStudentRestResponse(sId);
            return restResponse.IsSuccessStatusCode;
        }

        public async Task<RestResponse> DeleteStudentRestResponse(int sId)
        {
            var request = new RestRequest(BaseRoute + "{sId}").AddUrlSegment("sId", sId);
            request.Method = Method.Delete;

            var restResponse = await _restClient.ExecuteAsync(request);
            return restResponse;
        }

        // Patch
        public async Task<StudentDto?> PatchStudent(int sId, PatchStudentDto student, byte[] rowVersion)
        {
            var restResponse = await PatchStudentRestResponse(sId, student, rowVersion);
            return restResponse.Data;
        }

        public async Task<RestResponse<StudentDto?>> PatchStudentRestResponse(int sId, PatchStudentDto student, byte[] rowVersion)
        {
            var request = new RestRequest(BaseRoute + "{sId}").AddUrlSegment("sId", sId).AddBody(student.GetObjectToSerialize());
            request.Method = Method.Patch;
            SetIfMatchHeader(request, rowVersion);

            var restResponse = await _restClient.ExecuteAsync<StudentDto?>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }

        // Get Students
        //TODO add method using the DTO query type 
        public async Task<PagedList<StudentDto>?> GetStudents(int? sId = null, string? name = null, GPAQueryDto? gpaQuery = null,
            SortOrder? orderBy = null, string? sortColumn = null, int? page = null, int? pageSize = null)
        {
            var restResponse = await GetStudentsRestResponse(sId, name, gpaQuery, orderBy, sortColumn, page, pageSize);
            return restResponse.Data;
        }

        public async Task<RestResponse<PagedList<StudentDto>>> GetStudentsRestResponse(int? sId = null, string? name = null, GPAQueryDto? gpaQuery = null,
            SortOrder? orderBy = null, string? sortColumn = null, int? page = null, int? pageSize = null)
        {
            var request = new RestRequest(BaseRoute);
            if (name is not null)
            {
                request.AddQueryParameter(APIConstants.Student.Name, name);
            }
            if (sId is not null)
            {
                request.AddQueryParameter(APIConstants.Student.SId, sId.Value);
            }
            if (gpaQuery?.GPA is not null)
            {
                if (gpaQuery.GPA.Lt is not null)
                {
                    request.AddQueryParameter($"{APIConstants.Student.GPA}.{APIConstants.Query.Lt}", gpaQuery.GPA.Lt.Value);
                }
                if (gpaQuery.GPA.Gt is not null)
                {
                    request.AddQueryParameter($"{APIConstants.Student.GPA}.{APIConstants.Query.Gt}", gpaQuery.GPA.Gt.Value);
                }
                if (gpaQuery.GPA.Eq is not null)
                {
                    request.AddQueryParameter($"{APIConstants.Student.GPA}.{APIConstants.Query.Eq}", gpaQuery.GPA.Eq.Value);
                }
                if (gpaQuery.GPA.IsNull is not null)
                {
                    request.AddQueryParameter($"{APIConstants.Student.GPA}.{APIConstants.Query.IsNull}", gpaQuery.GPA.IsNull.ToString());
                }
            }
            if(orderBy is not null)
            {
                request.AddQueryParameter($"{APIConstants.Query.OrderBy}", orderBy.ToString());
            }
            if(sortColumn is not null)
            {
                request.AddQueryParameter($"{APIConstants.Query.SortColumn}", sortColumn.ToString());
            }
            if(page is not null)
            {
                request.AddQueryParameter($"{APIConstants.Query.Page}", page.ToString());
            }
            if(pageSize is not null)
            {
                request.AddQueryParameter($"{APIConstants.Query.PageSize}", pageSize.ToString());
            }

            var restResponse = await _restClient.ExecuteGetAsync<PagedList<StudentDto>>(request);
            if (!restResponse.IsSuccessStatusCode)
            {
                restResponse.Data = null;
            }
            return restResponse;
        }

        private void SetIfMatchHeader(RestRequest restRequest, byte[] rowVersion)
        {
            restRequest.AddHeader(HeaderNames.IfMatch, Convert.ToBase64String(rowVersion));
        }

        private void SetRowVersionFromETagHeader(RestResponse<StudentDto> restResponse)
        {
            if (restResponse.Data is not null)
            {
                var rowVer = restResponse.GetHeaderValue(HeaderNames.ETag);
                if (rowVer is not null)
                {
                    restResponse.Data.RowVersion = Convert.FromBase64String(rowVer);
                }
            }
        }
    }
}
