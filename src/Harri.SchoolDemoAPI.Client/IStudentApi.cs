﻿using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
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
        Task<PagedList<StudentDto>?> GetStudents(int? sId = null, string? name = null, GPAQueryDto? gpaQuery = null, SortOrder? orderBy = null, string? sortColumn = null, int? page = null, int? pageSize = null);
        Task<RestResponse<PagedList<StudentDto>>> GetStudentsRestResponse(int? sId = null, string? name = null, GPAQueryDto? gpaQuery = null, SortOrder? orderBy = null, string? sortColumn = null, int? page = null, int? pageSize = null);
        Task<StudentDto?> PatchStudent(int sId, PatchStudentDto student, byte[] rowVersion);
        Task<RestResponse<StudentDto?>> PatchStudentRestResponse(int sId, PatchStudentDto student, byte[] rowVersion);
        Task<bool> UpdateStudent(int sId, UpdateStudentDto student, byte[] rowVersion);
        Task<RestResponse> UpdateStudentRestResponse(int sId, UpdateStudentDto student, byte[] rowVersion);
    }
}