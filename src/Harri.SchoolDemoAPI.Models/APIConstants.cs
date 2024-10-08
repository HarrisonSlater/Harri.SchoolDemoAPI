﻿namespace Harri.SchoolDemoAPI.Models
{
    public static class APIConstants
    {
        public static class Student
        {
            public const string SId = "sId";
            public const string Name = "name";
            public const string GPA = "GPA";
        }
        public static class School
        {
            public const string SchoolId = "schoolId";
            public const string SchoolName = "schoolName";
            public const string State = "state";
            public const string Enrollment = "enrollment";
        }
        public static class Application
        {
            public const string ApplicationId = "applicationId";
            public const string Major = "major";
            public const string Decision = "decision";
        }

        public static class Query
        {
            public const string GpaQuery = "gpaQuery"; //GPAQueryDto
            public const string Lt = "lt";
            public const string Gt = "gt";
            public const string Eq = "eq";
            public const string IsNull = "isNull";
            public const string OrderBy = "orderBy";
            public const string SortColumn = "sortColumn";

            public const string Page = "page";
            public const string PageSize = "pageSize";
        }
    }
}
