using System.Text;
using System.Text.Json;
using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
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
            this.providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["a student with sId exists"] = this.EnsureStudentExists,
                ["a student with sId does not exist"] = this.EnsureStudentDoesNotExist,
                ["a student with sId {sIdNew} will be created"] = this.EnsureStudentWillBeCreated,
                ["a student with sId exists and will be updated"] = this.EnsureStudentWillBeUpdated,
                ["a student with sId will be updated"] = this.EnsureStudentWillBeUpdated,
                ["no student will be updated"] = this.EnsureNoStudentWillBeUpdated,
                ["no student will be deleted"] = this.EnsureNoStudentWillBeDeleted,
                ["a student with sId exists and will be deleted"] = this.EnsureStudentWillBeDeleted,
                ["a student with sId does not exist and will not be deleted"] = this.EnsureStudentDoesNotExistAndWillBeNotDeleted,
                ["a student with sId exists but can not be deleted"] = this.EnsureStudentHasConflictAndCanNotNotDeleted,
                ["some students exist"] = this.EnsureSomeStudentsExist,
                ["no students exist"] = this.EnsureNoStudentsExist,
                ["some students exist for querying"] = this.EnsureStudentsExistForQuerying,
                ["no students exist for querying"] = this.EnsureNoStudentsExistForQuerying,
                ["the api returns a 500 internal server error"] = this.TheApiReturnsA500InternalServerError,
                ["the api is healthy"] = this.TheApiIsHealthy,
                ["the api is unhealthy"] = this.TheApiIsUnhealthy
            };
        }

        private Task EnsureStudentExists(IDictionary<string, object> parameters)
        {
            var studentToMock = GetStateObject<StudentDto>(parameters);

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
            var expectedUpdatedStudent = GetStateObject<UpdateStudentDto>(parameters);

            TestStartup.MockStudentRepo.Setup(s => s.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>()))
            .Returns(Task.FromResult(true))
            .Callback<int, UpdateStudentDto>((id, us) =>
            {
                id.Should().Be(sId?.GetInt32());
                us.Should().BeEquivalentTo(expectedUpdatedStudent);
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
                .Returns(Task.FromResult((bool?)true))
                .Callback<int>(sId => sId.Should().Be(sId));

            return Task.CompletedTask;
        }

        private Task EnsureStudentDoesNotExistAndWillBeNotDeleted(IDictionary<string, object> parameters)
        {
            var sId = GetStateObject<int>(parameters);
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)false))
                .Callback<int>(sId => sId.Should().Be(sId));
            return Task.CompletedTask;

        }

        private Task EnsureStudentHasConflictAndCanNotNotDeleted(IDictionary<string, object> parameters)
        {
            var sId = GetStateObject<int>(parameters);
            TestStartup.MockStudentRepo.Setup(s => s.DeleteStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((bool?)null))
                .Callback<int>(sId => sId.Should().Be(sId));
            return Task.CompletedTask;

        }

        private static List<StudentDto> MockStudentsToReturn =
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

        private static PagedList<StudentDto> MockPagedList = new()
        {
            Items = MockStudentsToReturn.ToList(),
            Page = 1,
            PageSize = 3,
            TotalCount = 3
        };


        private Task EnsureSomeStudentsExist(IDictionary<string, object> parameters)
        {
            TestStartup.MockStudentRepo.Setup(s => s.GetStudents(It.IsAny<GetStudentsQueryDto>()))
                .Returns(Task.FromResult(MockPagedList));

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
                .Returns(Task.FromResult(MockPagedList))
                .Callback<GetStudentsQueryDto>((queryDto) =>
                {
                    //TODO simplify
                    queryDto.SId.Should().Be(studentQueryDto.SId);
                    queryDto.Name.Should().Be(studentQueryDto.Name);
                    queryDto.GPAQueryDto.Should().BeEquivalentTo(studentQueryDto.GPAQueryDto);
                    queryDto.OrderBy.Should().Be(studentQueryDto.OrderBy ?? APIDefaults.Query.OrderBy);
                    queryDto.SortColumn.Should().Be(studentQueryDto.SortColumn.IsNullOrEmpty() ? APIDefaults.Query.SortColumn : studentQueryDto.SortColumn);
                    queryDto.Page.Should().Be(studentQueryDto.Page ?? APIDefaults.Query.Page);
                    queryDto.PageSize.Should().Be(studentQueryDto.PageSize ?? APIDefaults.Query.PageSize);
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
