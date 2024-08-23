using System.Text;
using System.Text.Json;
using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    /// <summary>
    /// Middleware for handling provider state requests
    /// base file from: https://github.com/pact-foundation/pact-net/blob/master/samples/OrdersApi/Provider.Tests/ProviderStateMiddleware.cs
    /// </summary>
    public class ProviderStateMiddleware
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> providerStates;
        private readonly RequestDelegate _next;
        //private readonly IStudentRepository _studentRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="ProviderStateMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request delegate</param>
        /// <param name="orders">Orders repository for actioning provider state requests</param>
        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            //_studentRepository = studentRepository;

            this.providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["a student with sId {sId} exists"] = this.EnsureStudentExists,
                ["a student with sId {sId} does not exist"] = this.EnsureStudentDoesNotExist,
                ["a student with sId {sIdNew} will be created"] = this.EnsureStudentWillBeCreated,
                ["a student with sId {sId} exists and will be updated"] = this.EnsureStudentWillBeUpdated,
                ["a student with sId {sId} will be updated"] = this.EnsureStudentWillBeUpdated,
                ["no student will be updated"] = this.EnsureNoStudentWillBeUpdated,
                ["no student will be deleted"] = this.EnsureNoStudentWillBeDeleted,
                ["a student with sId {sId} exists and will be deleted"] = this.EnsureStudentWillBeDeleted,
                ["a student with sId {sId} does not exist and will not be deleted"] = this.EnsureStudentDoesNotExistAndWillBeNotDeleted,
                ["a student with sId {sId} exists but can not be deleted"] = this.EnsureStudentHasConflictAndCanNotNotDeleted,
                ["some students exist"] = this.EnsureSomeStudentsExist,
                ["no students exist"] = this.EnsureNoStudentsExist,
                ["some students exist for querying"] = this.EnsureStudentsExistForQuerying,
                ["no students exist for querying"] = this.EnsureNoStudentsExistForQuerying,
                ["the api returns a 500 internal server error"] = this.TheApiReturnsA500InternalServerError,
                ["the api is healthy"] = this.TheApiIsHealthy,
                ["the api is unhealthy"] = this.TheApiIsUnhealthy
                //["a student with sId {sId} will be patched"] = this.EnsureStudentWillBePatched

            };
        }

        /// <summary>
        /// Ensure an event exists
        /// </summary>
        /// <param name="parameters">Event parameters</param>
        /// <returns>Awaitable</returns>
        private Task EnsureStudentExists(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            var name = (JsonElement?)parameters["name"];
            var gpa = (JsonElement?)parameters["GPA"];

            var studentToMock = new StudentDto()
            {
                SId = sId?.GetInt32(),
                Name = name?.GetString(),
                GPA = gpa?.GetDecimal()
            };

            TestStartup.MockStudentRepo.Setup(s => s.GetStudent(It.IsAny<int>())).Returns(Task.FromResult((StudentDto?)studentToMock));
            return Task.CompletedTask;

        }

        private Task EnsureStudentDoesNotExist(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudent(It.IsAny<int>())).Returns(Task.FromResult<StudentDto?>(null));
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>())).Returns(Task.FromResult(false));
            return Task.CompletedTask;

        }

        private Task EnsureNoStudentWillBeUpdated(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>())).Throws(new Exception("UpdateStudent should not be called"));
            return Task.CompletedTask;

        }

        private Task EnsureNoStudentWillBeDeleted(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>())).Throws(new Exception("DeleteStudent should not be called"));
            return Task.CompletedTask;

        }

        private Task EnsureStudentWillBeUpdated(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            var name = (JsonElement?)parameters["name"];
            var gpa = (JsonElement?)parameters["GPA"];

            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>()))
            .Returns(Task.FromResult(true))
            .Callback<int, UpdateStudentDto>((id, ns) =>
            {
                id.Should().Be(sId?.GetInt32());
                ns.Name.Should().Be(name?.ToString());
                ns.GPA.Should().Be(gpa?.GetDecimal());
            });

            return Task.CompletedTask;
        }

        private Task EnsureStudentWillBeCreated(IDictionary<string, object> parameters)
        {
            var sIdNew = (JsonElement?)parameters["sIdNew"];
            var name = (JsonElement?)parameters["name"];
            var gpa = (JsonElement?)parameters["GPA"];

            TestStartup.MockStudentRepo.Setup(s => s.AddStudent(It.IsAny<NewStudentDto>()))
                .Returns(Task.FromResult(sIdNew.Value.GetInt32()))
                .Callback<NewStudentDto>(ns =>
                {
                    ns.Name.Should().Be(name.ToString());
                    ns.GPA.Should().Be(gpa?.GetDecimal());
                });
            return Task.CompletedTask;

        }

        private Task EnsureStudentWillBeDeleted(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)true))
                .Callback<int>(sId =>
                {
                    sId.Should().Be(sId);
                });

            return Task.CompletedTask;

        }

        private Task EnsureStudentDoesNotExistAndWillBeNotDeleted(IDictionary<string, object> parameters)
        {

            var sId = (JsonElement?)parameters["sId"];
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)false))
                .Callback<int>(sId =>
                {
                    sId.Should().Be(sId);
                });
            return Task.CompletedTask;

        }

        private Task EnsureStudentHasConflictAndCanNotNotDeleted(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)null))
                .Callback<int>(sId =>
                {
                    sId.Should().Be(sId);
                });
            return Task.CompletedTask;

        }
        private List<StudentDto> _mockStudentsToReturn =
        [
            new StudentDto()
            {
                SId = 1,
                Name = "Test student 1",
                GPA = 3.99m
            },
            new StudentDto()
            {
                SId = 2,
                Name = "Test student 2",
                GPA = 3.89m
            },
            new StudentDto()
            {
                SId = 3,
                Name = "Test student 3",
                GPA = 3.79m
            }
        ];

        private Task EnsureSomeStudentsExist(IDictionary<string, object> parameters)
        {

            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>(), It.IsAny<SortOrder?>()))
                .Returns(Task.FromResult(_mockStudentsToReturn));

            return Task.CompletedTask;
        }

        private Task EnsureNoStudentsExist(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>(), It.IsAny<SortOrder?>()))
                .Returns(Task.FromResult(new List<StudentDto>()));

            return Task.CompletedTask;
        }

        private Task EnsureNoStudentsExistForQuerying(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>(), It.IsAny<SortOrder?>()))
                .Returns(Task.FromResult(new List<StudentDto>()));

            return Task.CompletedTask;
        }

        //TODO refactor
        private Task EnsureStudentsExistForQuerying(IDictionary<string, object> parameters)
        {
            //Either parameter could be null
            string? name = ((JsonElement?)parameters["name"])?.ToString();
            var gpaQueryString = (JsonElement?)parameters["gpaQuery"];

            string? orderBy = null;
            if (parameters.ContainsKey("orderBy"))
            {
                orderBy = ((JsonElement?)parameters["orderBy"])?.ToString();
            }

            GPAQueryDto? expectedGpaQuery = new GPAQueryDto() { GPA = null };
            if (gpaQueryString is not null)
            {
                expectedGpaQuery = JsonSerializer.Deserialize<GPAQueryDto>(gpaQueryString.ToString() ?? "");
            }
            SortOrder? expectedOrderBy = null;
            if (orderBy is not null)
            {
                SortOrder parsedOrderBy; 
                if (Enum.TryParse(orderBy, true, out parsedOrderBy))
                {
                    expectedOrderBy = parsedOrderBy;
                }

                //expectedOrderBy = JsonSerializer.Deserialize<SortOrder?>(orderBy ?? "null");
            }

            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>(), It.IsAny<SortOrder?>()))
                .Returns(Task.FromResult(_mockStudentsToReturn))
                .Callback<string, GPAQueryDto, SortOrder?>((nameParam, gpaQueryDtoParam, orderByParam) =>
                {
                    nameParam.Should().Be(name);
                    gpaQueryDtoParam.Should().BeEquivalentTo(expectedGpaQuery);
                    orderByParam.Should().Be(expectedOrderBy);
                });

            return Task.CompletedTask;
        }

        private Task TheApiReturnsA500InternalServerError(IDictionary<string, object> parameters)
        {
            var exceptionMessage = "Test Exception. Internal Server Error";
            var testException = new Exception(exceptionMessage);

            TestStartup.MockStudentRepo.Setup(s => s.AddStudent(It.IsAny<NewStudentDto>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.GetStudent(It.IsAny<int>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>(), It.IsAny<SortOrder?>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<string>(), It.IsAny<GPAQueryDto>(), It.IsAny<SortOrder?>())).Throws(testException);

            return Task.CompletedTask;
        }

        private Task TheApiIsHealthy(IDictionary<string, object> parameters)
        {
            TestStartup.MockHealthCheck.Setup(s => s.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(
                    HealthCheckResult.Healthy()
                ));
            return Task.CompletedTask;
        }

        private Task TheApiIsUnhealthy(IDictionary<string, object> parameters)
        {
            TestStartup.MockHealthCheck.Setup(s => s.CheckHealthAsync(It.IsAny<HealthCheckContext>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(
                    HealthCheckResult.Unhealthy(description:"Some error description", exception:new Exception("Some error description"))
                ));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="context">Request context</param>
        /// <returns>Awaitable</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await _next.Invoke(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            if (context.Request.Method == HttpMethod.Post.ToString())
            {
                string jsonRequestBody;

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                try
                {
                    ProviderState? providerState = JsonSerializer.Deserialize<ProviderState>(jsonRequestBody, Options);
                    
                    if (!string.IsNullOrEmpty(providerState?.State))
                    {
                        await this.providerStates[providerState.State].Invoke(providerState.Params);
                    }
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Failed to deserialise JSON provider state body:");
                    await context.Response.WriteAsync(jsonRequestBody);
                    await context.Response.WriteAsync(string.Empty);
                    await context.Response.WriteAsync(e.ToString());
                }
            }
        }
    }
}
