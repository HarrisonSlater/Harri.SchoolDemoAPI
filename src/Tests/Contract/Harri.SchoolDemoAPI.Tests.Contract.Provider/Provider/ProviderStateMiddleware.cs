using System.Text;
using System.Text.Json;
using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Results;
using Harri.SchoolDemoAPI.Tests.Common;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PactNet.Exceptions;

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

        /// <summary>
        /// Initialises a new instance of the <see cref="ProviderStateMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request delegate</param>
        /// <param name="orders">Orders repository for actioning provider state requests</param>
        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["a student with sId exists"] = EnsureStudentExists,
                ["a student with sId does not exist"] = EnsureStudentDoesNotExist,
                ["a student with sId {sIdNew} will be created"] = EnsureStudentWillBeCreated,
                ["a student with sId exists and will be updated"] = EnsureStudentWillBeUpdated,
                ["a student with sId will be updated"] = EnsureStudentWillBeUpdated,
                ["a student with sId will be patched"] = EnsureStudentWillBePatched,
                ["no student will be updated"] = EnsureNoStudentWillBeUpdated,
                ["a student with sId exists but a race condition occurs at the database level"] = EnsureUpdatingStudentReturnsFailureResultConflict,
                ["a student with sId exists but was updated before us"] = EnsureUpdatingStudentReturnsFailureResultRowVersionMismatch,
                ["a student with sId exists but our request is missing an If-Match header"] = EnsureNoStudentWillBeUpdated,
                ["no student will be deleted"] = EnsureNoStudentWillBeDeleted,
                ["a student with sId exists and will be deleted"] = EnsureStudentWillBeDeleted,
                ["a student with sId does not exist and will not be deleted"] = EnsureStudentDoesNotExistAndWillBeNotDeleted,
                ["a student with sId exists but can not be deleted"] = EnsureStudentHasConflictAndCanNotNotDeleted,
                ["some students exist"] = EnsureSomeStudentsExist,
                ["no students exist"] = EnsureNoStudentsExist,
                ["some students exist for querying"] = EnsureStudentsExistForQuerying,
                ["some students exist for querying across multiple pages"] = EnsureStudentsExistForQueryingAcrossMultiplePages,
                ["no students exist for querying"] = EnsureNoStudentsExistForQuerying,
                ["the api returns a 500 internal server error"] = TheApiReturnsA500InternalServerError,
                ["the api is healthy"] = TheApiIsHealthy,
                ["the api is unhealthy"] = TheApiIsUnhealthy
            };
        }

        private Task EnsureStudentExists(IDictionary<string, object> parameters)
        {
            var studentToMock = GetStateObject<StudentDto>(parameters);
            studentToMock.RowVersion = MockStudentTestFixture.MockRowVersion;

            TestStartup.MockStudentRepo.Setup(s => s.GetStudent(It.IsAny<int>())).Returns(Task.FromResult((StudentDto?)studentToMock));
            return Task.CompletedTask;
        }

        private Task EnsureStudentDoesNotExist(IDictionary<string, object> parameters)
        {
            EnsureUpdatingStudentReturnsFailureResult(Result.Failure(StudentErrors.StudentNotFound.Error(0)));
            return Task.CompletedTask;
        }

        private Task EnsureNoStudentWillBeUpdated(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>(), It.IsAny<byte[]>())).Throws(new Exception("UpdateStudent should not be called"));
            return Task.CompletedTask;
        }

        private Task EnsureUpdatingStudentReturnsFailureResultConflict(IDictionary<string, object> parameters)
        {
            EnsureUpdatingStudentReturnsFailureResult(Result.Failure(StudentErrors.StudentUpdateConflict.Error(0)));
            return Task.CompletedTask;
        }

        private Task EnsureUpdatingStudentReturnsFailureResultRowVersionMismatch(IDictionary<string, object> parameters)
        {
            EnsureUpdatingStudentReturnsFailureResult(Result.Failure(StudentErrors.StudentRowVersionMismatch.Error(0)));
            return Task.CompletedTask;
        }

        private void EnsureUpdatingStudentReturnsFailureResult(Result resultToReturn)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudent(It.IsAny<int>())).Returns(Task.FromResult<StudentDto?>(null));
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>(), It.IsAny<byte[]>())).Returns(Task.FromResult(resultToReturn));
            TestStartup.MockStudentRepo.Setup(s => s.PatchStudent(It.IsAny<int>(), It.IsAny<PatchStudentDto>(), It.IsAny<byte[]>())).Returns(Task.FromResult(ResultWith<StudentDto>.FromResult(resultToReturn)));
        }

        private Task EnsureNoStudentWillBeDeleted(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>())).Throws(new Exception("DeleteStudent should not be called"));
            return Task.CompletedTask;
        }

        private Task EnsureStudentWillBeUpdated(IDictionary<string, object> parameters)
        {
            var sId = (JsonElement?)parameters["sId"];
            var expectedUpdatedStudent = GetStateObject<UpdateStudentDto>(parameters);

            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>(), It.IsAny<byte[]>()))
            .Returns(Task.FromResult(Result.Success()))
            .Callback<int, UpdateStudentDto, byte[]>((id, us, rowVersion) =>
            {
                id.Should().Be(sId?.GetInt32());
                us.Should().BeEquivalentTo(expectedUpdatedStudent);
                rowVersion.Should().BeEquivalentTo(MockStudentTestFixture.MockRowVersion);
            });

            return Task.CompletedTask;
        }
        private Task EnsureStudentWillBePatched(IDictionary<string, object> parameters)
        {
            var sIdString = (JsonElement?)parameters["sId"];
            var sId = sIdString?.GetInt32();
            var expectedUpdatedStudent = GetStateObject<PatchStudentDto>(parameters);

            TestStartup.MockStudentRepo.Setup(s => s.PatchStudent(It.IsAny<int>(), It.IsAny<PatchStudentDto>(), It.IsAny<byte[]>()))
            .Returns(Task.FromResult(ResultWith<StudentDto>.Success(new StudentDto() { SId = sId, Name = expectedUpdatedStudent.Name, GPA = expectedUpdatedStudent.GPA })))
            .Callback<int, PatchStudentDto, byte[]>((id, patchDto, rowVersion) =>
            {
                id.Should().Be(sId);
                patchDto.Should().BeEquivalentTo(expectedUpdatedStudent);
                rowVersion.Should().BeEquivalentTo(MockStudentTestFixture.MockRowVersion);
            });

            return Task.CompletedTask;
        }

        private Task EnsureStudentWillBeCreated(IDictionary<string, object> parameters)
        {
            var sIdNew = (JsonElement?)parameters["sIdNew"];
            var expectedNewStudent = GetStateObject<NewStudentDto>(parameters);

            TestStartup.MockStudentRepo.Setup(s => s.AddStudent(It.IsAny<NewStudentDto>()))
                .Returns(Task.FromResult(sIdNew.Value.GetInt32()))
                .Callback<NewStudentDto>(ns => ns.Should().BeEquivalentTo(expectedNewStudent));
            return Task.CompletedTask;
        }

        private Task EnsureStudentWillBeDeleted(IDictionary<string, object> parameters)
        {

            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult(Result.Success()))
                .Callback<int>(sId => sId.Should().Be(sId));

            return Task.CompletedTask;
        }

        private Task EnsureStudentDoesNotExistAndWillBeNotDeleted(IDictionary<string, object> parameters)
        {
            var sId = GetStateObject<int>(parameters);
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult(Result.Failure(StudentErrors.StudentNotFound.Error(sId))))
                .Callback<int>(sId => sId.Should().Be(sId));
            return Task.CompletedTask;

        }

        private Task EnsureStudentHasConflictAndCanNotNotDeleted(IDictionary<string, object> parameters)
        {
            var sId = GetStateObject<int>(parameters);
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult(Result.Failure(StudentErrors.StudentDeleteConflict.Error(sId))))
                .Callback<int>(sId => sId.Should().Be(sId));
            return Task.CompletedTask;

        }

        private Task EnsureSomeStudentsExist(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .Returns(Task.FromResult(MockStudentTestFixture.MockPagedList));

            return Task.CompletedTask;
        }

        private Task EnsureNoStudentsExist(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .Returns(Task.FromResult(new PagedList<StudentDto>()));

            return Task.CompletedTask;
        }

        private Task EnsureNoStudentsExistForQuerying(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .Returns(Task.FromResult(new PagedList<StudentDto>()));

            return Task.CompletedTask;
        }

        private Task EnsureStudentsExistForQuerying(IDictionary<string, object> parameters)
        {
            var studentQueryDto = GetStateObject<GetStudentsQueryDto>(parameters);

            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .Returns(Task.FromResult(MockStudentTestFixture.MockPagedList))
                .Callback(GetAssertionCallback(studentQueryDto));

            return Task.CompletedTask;
        }

        private Task EnsureStudentsExistForQueryingAcrossMultiplePages(IDictionary<string, object> parameters)
        {
            var studentQueryDto = GetStateObject<GetStudentsQueryDto>(parameters);

            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .Returns(Task.FromResult(MockStudentTestFixture.MockPagedListAcrossMultiplePages))
                .Callback(GetAssertionCallback(studentQueryDto));

            return Task.CompletedTask;
        }

        // Asserts that the mock is called with expected arguments (that they are parsed correctly by the provider)
        private static Action<GetStudentsQueryDto> GetAssertionCallback(GetStudentsQueryDto studentQueryDto) 
        {
            return (queryDto) =>
            {
                queryDto.Should().BeEquivalentTo(studentQueryDto, (options) =>
                {
                    return options.Excluding(dto => dto.OrderBy)
                    .Excluding(dto => dto.SortColumn)
                    .Excluding(dto => dto.Page)
                    .Excluding(dto => dto.PageSize);
                });

                queryDto.OrderBy.Should().Be(studentQueryDto.OrderBy ?? APIDefaults.Query.OrderBy);
                queryDto.SortColumn.Should().Be(studentQueryDto.SortColumn.IsNullOrEmpty() ? APIDefaults.Query.SortColumn : studentQueryDto.SortColumn);
                queryDto.Page.Should().Be(studentQueryDto.Page ?? APIDefaults.Query.Page);
                queryDto.PageSize.Should().Be(studentQueryDto.PageSize ?? APIDefaults.Query.PageSize);
            };
        }

        private Task TheApiReturnsA500InternalServerError(IDictionary<string, object> parameters)
        {
            var exceptionMessage = "Test Exception. Internal Server Error";
            var testException = new Exception(exceptionMessage);

            TestStartup.MockStudentRepo.Setup(s => s.AddStudent(It.IsAny<NewStudentDto>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.GetStudent(It.IsAny<int>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>(), It.IsAny<byte[]>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.PatchStudent(It.IsAny<int>(), It.IsAny<PatchStudentDto>(), It.IsAny<byte[]>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>())).Throws(testException);
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>())).Throws(testException);

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
        /// Deserialises the provider state object set by the Given method extension in consumer contract tests.  
        /// </summary>
        /// <remarks>State object is used to set up the provider state and may be used for mocking or asserting on in provider tests. See <see cref="Contract.Consumer.PactBuilderExtensions"/></remarks>
        /// <returns></returns>
        /// <exception cref="ProviderStateMiddlewareArgumentException"></exception>
        /// <exception cref="PactFailureException"></exception>
        private static T GetStateObject<T>(IDictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey("stateObject")) throw new ProviderStateMiddlewareArgumentException("ProviderStateMiddleware cannot find 'stateObject' key for provider state setup, please configure the contract test");
            if (!parameters.ContainsKey("stateObjectType")) throw new ProviderStateMiddlewareArgumentException("'stateObjectType' key not set. Something went wrong.");

            Type typeToDeserialize = typeof(T);
            var stateObjectType = ((JsonElement?)parameters["stateObjectType"]).ToString();
            if (stateObjectType is null || stateObjectType != typeToDeserialize.Name) 
            {
                // Misconfiguration between consumer test and provider test
                throw new PactFailureException("'stateObjectType' type does not match the type being deserialized to in provider state setup, please configure the contract test or provider state middleware");
            }

            var stateObject = JsonSerializer.Deserialize<T>(((JsonElement?)parameters["stateObject"]).ToString()!);
            if (stateObject is null) throw new ArgumentException("Deserialised stateObject cannot be null");

            return stateObject;
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
